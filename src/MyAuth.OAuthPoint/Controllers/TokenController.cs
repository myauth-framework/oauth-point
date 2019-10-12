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
            var trp = new TokenRequestProcessor(tokenRequest);

            if (trp.CheckState() is ErrorTokenResponse badState)
                return BadRequest(badState);
            if (await trp.LoadAndCheckLoginRequest(_loginRegistry, _refreshTokenRegistry) is ErrorTokenResponse badRequest)
                return BadRequest(badRequest);

            var refreshToken = RefreshToken.Generate(_options.RefreshTokenLifeTimeDays);

            await trp.RegisterRefreshToken(refreshToken, _refreshTokenRegistry);

            var succResp = trp.CreateSuccessResponse(
                refreshToken, 
                _options.Secret,
                _options.Issuer,
                _options.AccessTokenLifeTimeMin);


            return Ok(succResp);
        }
    }
}