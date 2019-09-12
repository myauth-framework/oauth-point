using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab.Config;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILoginRegistry _loginRegistry;
        private readonly IConfiguration _configuration;

        public TokenController(ILoginRegistry loginRegistry, IConfiguration configuration)
        {
            _loginRegistry = loginRegistry;
            _configuration = configuration;
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

            var tokenSettings = _configuration.GetNode<TokenSettingsConfig>();
            
            var accessTokenBuilder = new AccessTokenBuilder(tokenSettings.Secret)
            {
                Issuer = tokenSettings.Issuer,
                LifeTimeMin = tokenSettings.AccessTokenLifeTimeMin
            };

            var refreshToken = RefreshTokenGenerator.Generate();
            
            var succResp = new SuccessTokenResponse
            {
                AccessToken = accessTokenBuilder.Build(loginRequest),
                ExpiresIn = tokenSettings.AccessTokenLifeTimeMin * 60,
                RefreshToken = refreshToken
            };
            
            return Ok(succResp);
        }
    }
}