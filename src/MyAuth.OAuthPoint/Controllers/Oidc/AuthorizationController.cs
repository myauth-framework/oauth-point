using System;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Controllers.Oidc
{
    [Route("oidc/v1/authorization")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly AuthOptions _options;
        private readonly IDslLogger _log;
        private readonly AuthorizationRequestValidator _reqValidator;

        public AuthorizationController(
            ILoginService loginService, 
            IOptions<AuthOptions> options, 
            ILogger<AuthorizationController> logger,
            IStringLocalizer<AuthorizationRequestValidator> localizer)
        {
            _loginService = loginService;
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

                if (LoginSessionCookie.TryLoad(Request, out var authCookie))
                {
                    var lSession = await _loginService.GetLoginSessionAsync(authCookie.SessionId);
                    if (lSession != null && lSession.InitDetails.Error == null)
                    {
                        return UrlRedirector.RedirectSuccessCallback(lSession.InitDetails.RedirectUri, lSession.InitDetails.AuthorizationCode, lSession.InitDetails.State);
                    }
                }

                var loginId = await _loginService.CreateLoginSessionAsync(request);

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

                return UrlRedirector.RedirectError(
                    request.RedirectUri ?? _options.DefaultErrorEndpoint, 
                    e.Reason, 
                    e.Message, 
                    request.State);
            }
        }
    }
}
