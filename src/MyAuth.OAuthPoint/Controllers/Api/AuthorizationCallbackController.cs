using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers.Api
{
    [Route("api/v1/authorization-callback")]
    [ApiController]
    public class AuthorizationCallbackController : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly AuthOptions _options;

        public AuthorizationCallbackController(
            ISessionProvider sessionProvider, 
            IOptions<AuthOptions> options)
        {
            _sessionProvider = sessionProvider;
            _options = options.Value;
        }

        [HttpGet]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionExpiredException), HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromQuery(Name = "login_session_id")] string sessId)
        {
            var foundSess = await _sessionProvider.ProvideOAuth2DetailsAsync(sessId);

            if (foundSess == null)
            {
                return NotFound("Login session not found");
            }

            if (foundSess.ErrorCode != AuthorizationRequestProcessingError.Undefined)
            {
                return UrlRedirector.RedirectError(foundSess.RedirectUri, foundSess.ErrorCode, foundSess.ErrorDescription, foundSess.State);
            }

            var authCookie = new LoginSessionCookie(sessId)
            {
                Expiry = TimeSpan.FromDays(_options.SessionExpiryDays)
            };
            authCookie.Save(Response);

            return UrlRedirector.RedirectSuccessCallback(foundSess.RedirectUri, foundSess.AuthorizationCode, foundSess.State);
        }
    }
}
