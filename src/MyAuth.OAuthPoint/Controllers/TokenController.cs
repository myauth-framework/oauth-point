using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IRefreshTokenRegistry _refreshTokenRegistry;

        public TokenController(
            IOptions<TokenIssuingOptions> options, 
            ILoginRegistry loginRegistry,
            IRefreshTokenRegistry refreshTokenRegistry)
        {
            _options = options.Value;
            _loginRegistry = loginRegistry;
            _refreshTokenRegistry = refreshTokenRegistry;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] TokenRequest tokenRequest)
        {
            var reqChecker = new TokenRequestChecker(tokenRequest);
            
            var errResp = reqChecker.CheckState();
            if (errResp != null)
                return BadRequest(errResp);
            
            var loginReqProvider = GetLoginRequestProvider();
            var loginRequest = await loginReqProvider.Provide();
            
            errResp = reqChecker.CheckLoginRequest(loginRequest);
            if (errResp != null)
                return BadRequest(errResp);

            var accessTokenBuilder = new AccessTokenBuilder(_options.Secret)
            {
                Issuer = _options.Issuer,
                LifeTimeMin = _options.AccessTokenLifeTimeMin
            };

            var refreshToken = RefreshToken.Generate(_options.RefreshTokenLifeTimeDays);

            var succResp = new SuccessTokenResponse
            {
                AccessToken = accessTokenBuilder.Build(loginRequest),
                ExpiresIn = _options.AccessTokenLifeTimeMin * 60,
                RefreshToken = refreshToken.Body 
            };

            await _refreshTokenRegistry.Register(refreshToken.Body, new RefreshTokenDescription
            {
                LoginRequest = loginRequest,
                NotAfter = refreshToken.NotAfter
            });
            
            return Ok(succResp);

            ILoginRequestProvider GetLoginRequestProvider()
            {
                switch (tokenRequest.GrantType)
                {
                    case TokenRequestChecker.AuthCodeGrantType:
                        return new AuthCodeBasedLoginRequestProvider(tokenRequest.AuthCode, _loginRegistry);
                    case TokenRequestChecker.RefreshTokenGrantType: 
                        return new RefreshTokenBasedLoginRequestProvider(tokenRequest.RefreshToken, _refreshTokenRegistry);
                    default:
                        throw new IndexOutOfRangeException("Unsupported grant type");
                }   
            }
        }
    }
}