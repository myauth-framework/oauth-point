using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Controllers.Oidc
{
    [Route("v1/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILoginSessionProvider _loginSessionProvider;
        private readonly ILoginSessionCreator _loginSessionCreator;
        private readonly AuthEndpointsOptions _options;
        private readonly IDslLogger _log;
        private readonly AuthorizationRequestValidator _reqValidator;

        public AuthorizationController(
            ILoginSessionProvider loginSessionProvider, 
            ILoginSessionCreator loginSessionCreator,
            IOptions<AuthEndpointsOptions> options, 
            ILogger<AuthorizationController> logger,
            IStringLocalizer<AuthorizationController> localizer)
        {
            _loginSessionProvider = loginSessionProvider;
            _loginSessionCreator = loginSessionCreator;
            _options = options.Value;
            _log = logger.Dsl();
            _reqValidator = new AuthorizationRequestValidator(localizer);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AuthorizationRequest request)
        {
            try
            {
                _reqValidator.Validate(request);

                if (LoginSessionCookie.TryLoad(Request, out var loginCookie))
                {
                    var foundSess = await _loginSessionProvider.ProvideOAuth2DetailsAsync(loginCookie.SessionId, request.ClientId);

                    if (foundSess == null)
                    {
                        _log.Debug("Active session not found by cookie")
                            .AndFactIs("session-id", loginCookie.SessionId)
                            .AndFactIs("request", request)
                            .Write();
                    }
                    else
                    {
                        if (foundSess.ClientId != request.ClientId ||
                            foundSess.RedirectUri != request.RedirectUri ||
                            foundSess.Scope != request.Scope)
                        {
                            _log.Warning("Session found by cookie but parameters mismatch")
                                .AndFactIs("session-id", loginCookie.SessionId)
                                .AndFactIs("request", request)
                                .AndFactIs("stored-client-id", foundSess.ClientId)
                                .AndFactIs("stored-redirect-uri", foundSess.RedirectUri)
                                .AndFactIs("stored-scope", foundSess.Scope)
                                .Write();
                        }
                        else
                        {
                            return UrlRedirector.RedirectSuccessCallback(foundSess.RedirectUri,
                                foundSess.AuthorizationCode, foundSess.State);
                        }
                    }
                }

                var loginId = await _loginSessionCreator.CreateLoginSessionAsync(request);

                return UrlRedirector.RedirectToLogin(_options.LoginEndpoint, loginId, request.ClientId);
            }
            catch (RedirectUriException e)
            {
                _log.Warning(e)
                    .AndFactIs("request", request)
                    .Write();

                return UrlRedirector.RedirectError(
                    _options.DefaultErrorEndpoint,
                    AuthorizationRequestProcessingError.InvalidRequest,
                    e.Message,
                    request.State);
            }
            catch (AuthorizationRequestProcessingException e)
            {
                var log = e.Reason == AuthorizationRequestProcessingError.ServerError 
                    ? _log.Error("Authorization request error") 
                    : _log.Warning("Authorization request error");

                log.AndFactIs("request", request)
                    .AndFactIs("error-code", e.Reason)
                    .AndFactIs("error-msg", e.Message)
                    .Write();

                return UrlRedirector.RedirectError(
                    request.RedirectUri ?? _options.DefaultErrorEndpoint, 
                    e.Reason, 
                    e.Message, 
                    request.State);
            }
        }
    }
}
