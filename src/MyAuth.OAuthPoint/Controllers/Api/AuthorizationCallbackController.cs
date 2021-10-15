using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Log;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers.Api
{
    [Route("api/v1/authorization-callback")]
    [ApiController]
    public class AuthorizationCallbackController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly AuthOptions _options;

        public AuthorizationCallbackController(
            ILoginService loginService, 
            IOptions<AuthOptions> options)
        {
            _loginService = loginService;
            _options = options.Value;
        }

        [HttpGet]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionExpiredException), HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromQuery(Name = "login_session_id")] string loginId)
        {
            var lSession = await _loginService.GetLoginSessionAsync(loginId);

            var sessState = lSession.InitDetails;

            if (sessState.Error != null && sessState.Error.Error != AuthorizationRequestProcessingError.Undefined)
            {
                return UrlRedirector.RedirectError(sessState.RedirectUri, sessState.Error.Error, sessState.Error.Description, sessState.State);
            }

            if (lSession.Expiry < DateTime.Now)
                throw new LoginSessionExpiredException()
                    .AndFactIs("session-id", loginId);

            var authCookie = new LoginSessionCookie(loginId)
            {
                Expiry = TimeSpan.FromDays(_options.LoginSessionExpiryDays)
            };
            authCookie.Save(Response);

            return UrlRedirector.RedirectSuccessCallback(sessState.RedirectUri, sessState.AuthorizationCode, sessState.State);
        }
    }
}
