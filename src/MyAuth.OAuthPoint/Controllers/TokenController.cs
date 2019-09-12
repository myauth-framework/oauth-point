using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly TokenIssuingOptions _options;
        private readonly ILoginRegistry _loginRegistry;

        public TokenController(IOptions<TokenIssuingOptions> options, ILoginRegistry loginRegistry)
        {
            _options = options.Value;
            _loginRegistry = loginRegistry;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] TokenRequest tokenRequest)
        {
            var loginReqProvider = new LoginRequestProviderWithCache(_loginRegistry);
            var reqChecker = new TokenRequestChecker(loginReqProvider);
            
            var errResp = await reqChecker.Check(tokenRequest);
            if (errResp != null)
                return BadRequest(errResp);

            var loginRequest = await loginReqProvider.Provide(tokenRequest.AuthCode);

            var accessTokenBuilder = new AccessTokenBuilder(_options.Secret)
            {
                Issuer = _options.Issuer,
                LifeTimeMin = _options.AccessTokenLifeTimeMin
            };

            var refreshToken = RefreshTokenGenerator.Generate();
            
            var succResp = new SuccessTokenResponse
            {
                AccessToken = accessTokenBuilder.Build(loginRequest),
                ExpiresIn = _options.AccessTokenLifeTimeMin * 60,
                RefreshToken = refreshToken
            };
            
            return Ok(succResp);
        }
    }
}