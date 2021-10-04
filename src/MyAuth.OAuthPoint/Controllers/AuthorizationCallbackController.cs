using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("authorization-callback")]
    [ApiController]
    public class AuthorizationCallbackController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IStringLocalizer<AuthorizationCallbackController> _localizer;
        private readonly AuthOptions _options;
        private readonly IDslLogger _log;

        public AuthorizationCallbackController(
            IAuthorizationService authorizationService, 
            IOptions<AuthOptions> options, 
            ILogger<AuthorizationController> logger,
            IStringLocalizer<AuthorizationCallbackController> localizer)
        {
            _authorizationService = authorizationService;
            _localizer = localizer;
            _options = options.Value;
            _log = logger.Dsl();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "login_session_id")] string loginId)
        {
            var lSession = _authorizationService.GetLoginSession(loginId);

            if (lSession == null)
            {
                _log.Warning("Login session not found")
                    .AndFactIs("login_id", loginId)
                    .Write();

                return UrlRedirector.RedirectDefaultError(_options.DefaultErrorEndpoint, _localizer["LoginSessionNotFound"]);
            }

            if (lSession.Error != null)
            {
                return UrlRedirector.RedirectCallbackError(lSession.RedirectUri, lSession.Error.Error, lSession.Error.Description, lSession.State);
            }

            var authCookie = new LoginSessionCookie(loginId);
            authCookie.Save(Response);

            return UrlRedirector.RedirectSuccessCallback(lSession.RedirectUri, lSession.AuthorizationCode, lSession.State);
        }
    }
}
