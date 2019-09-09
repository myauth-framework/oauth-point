using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILoginRegistry _loginRegistry;

        public TokenController(ILoginRegistry loginRegistry)
        {
            _loginRegistry = loginRegistry;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TokenRequest tokenRequest)
        {
            var loginReqProvider = new LoginRequestProviderWithCache(_loginRegistry);
            var reqChecker = new TokenRequestChecker(loginReqProvider);
            
            var checkError = await reqChecker.Check(tokenRequest);
            if (checkError != null)
                return BadRequest(checkError);

            var loginRequest = await loginReqProvider.Provide(tokenRequest.AuthCode);
            
            return Ok();
        }
    }
}