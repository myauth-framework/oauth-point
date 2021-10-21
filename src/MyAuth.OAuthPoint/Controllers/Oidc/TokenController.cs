using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Controllers.Oidc
{
    [Route("oidc/v1/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IStringLocalizer<TokenController> _localizer;

        public TokenController(
            ITokenService tokenService,
            IStringLocalizer<TokenController> localizer)
        {
            _tokenService = tokenService;
            _localizer = localizer;
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> RequestToken([FromForm] TokenRequest request)
        {
            if (!AuthenticationHeaderValue.TryParse(HttpContext.Request.Headers["Authorization"],
                out var authHeaderVal))
            {
                HttpContext.Response.Headers.Add("WWW-Authenticate", "Basic");

                return Unauthorized(new ErrorTokenResponse
                {
                    AuthError = TokenRequestProcessingError.InvalidClient,
                    Description = _localizer["Unauthorized"]
                });
            }
            
            if (authHeaderVal.Scheme != "Basic")
            {
                return BadRequest(new ErrorTokenResponse
                {
                    AuthError = TokenRequestProcessingError.InvalidClient,
                    Description = _localizer["UnsupportedAuthScheme"]
                });
            }

            if(!BasicAuthParser.TryParse(authHeaderVal.Parameter, out var clientId, out var clientPassword))
            {
                return BadRequest(new ErrorTokenResponse
                {
                    AuthError = TokenRequestProcessingError.InvalidClient,
                    Description = _localizer["CantParseCredentials"]
                });
            }

            try
            {
                var resp = await _tokenService.IssueAsync(clientId, clientPassword, request);

                return Ok(resp);
            }
            catch (TokenRequestProcessingException e)
            {
                return BadRequest(new ErrorTokenResponse
                {
                    AuthError = e.Reason,
                    Description = e.Message
                });
            }
        }
    }
}
