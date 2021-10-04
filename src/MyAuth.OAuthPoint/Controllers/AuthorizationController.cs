using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly AuthOptions _options;
        private readonly IDslLogger _log;
        private readonly AuthorizationRequestValidator _reqValidator;

        public AuthorizationController(
            IAuthorizationService authorizationService, 
            IOptions<AuthOptions> options, 
            ILogger<AuthorizationController> logger,
            IStringLocalizer<AuthorizationRequestValidator> localizer)
        {
            _authorizationService = authorizationService;
            _options = options.Value;
            _log = logger.Dsl();
            _reqValidator = new AuthorizationRequestValidator(localizer);
        }

        [HttpGet]
        public async Task<IActionResult> Get(AuthorizationRequest request)
        {
            try
            {
                _reqValidator.Validate(request);

                if (LoginSessionCookie.TryLoad(Request, out var authCookie))
                {
                    var lSession = _authorizationService.GetLoginSession(authCookie.SessionId);
                    if (lSession != null && lSession.Error == null)
                    {
                        return UrlRedirector.RedirectSuccessCallback(lSession.RedirectUri, lSession.AuthorizationCode, lSession.State);
                    }
                }

                var loginId = _authorizationService.CreateLoginSession();

                return UrlRedirector.RedirectToLogin(_options.LoginEndpoint, loginId);
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

                return request.RedirectUri != null
                    ? UrlRedirector.RedirectCallbackError(request.RedirectUri, e.Reason, e.Message, request.State)
                    : UrlRedirector.RedirectDefaultError(_options.DefaultErrorEndpoint, e.Message);
            }
        }
    }
}
