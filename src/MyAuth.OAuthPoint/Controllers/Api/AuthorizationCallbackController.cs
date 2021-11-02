using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers.Api
{
    [Route("v1/authorization-callback")]
    [ApiController]
    public class AuthorizationCallbackController : ControllerBase
    {
        private readonly ILoginSessionProvider _loginSessionProvider;
        private readonly AuthTimingsOptions _options;

        public AuthorizationCallbackController(
            ILoginSessionProvider loginSessionProvider, 
            IOptions<AuthTimingsOptions> options)
        {
            _loginSessionProvider = loginSessionProvider;
            _options = options.Value;
        }

        [HttpGet]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionExpiredException), HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromQuery(Name = "login_id")] string loginId, [FromQuery(Name = "client_id")] string clientId)
        {
            var foundSess = await _loginSessionProvider.ProvideOAuth2DetailsAsync(loginId, clientId, TokenSessionDbStatus.Ready);

            if (foundSess == null)
            {
                return NotFound("Login session not found");
            }

            if (foundSess.ErrorCode != AuthorizationRequestProcessingError.Undefined)
            {
                return UrlRedirector.RedirectError(foundSess.RedirectUri, foundSess.ErrorCode, foundSess.ErrorDescription, foundSess.State);
            }

            var authCookie = new LoginSessionCookie(loginId)
            {
                Expiry = TimeSpan.FromDays(_options.SessionExpiryDays)
            };
            authCookie.Save(Response);

            return UrlRedirector.RedirectSuccessCallback(foundSess.RedirectUri, foundSess.AuthorizationCode, foundSess.State);
        }
    }
}
