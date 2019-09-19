using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("refresh-tokens")]
    [ApiController]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IRefreshTokenRegistry _refreshTokenRegistry;

        /// <summary>
        /// Initializes a new instance of <see cref="RefreshTokensController"/>
        /// </summary>
        public RefreshTokensController(IRefreshTokenRegistry refreshTokenRegistry)
        {
            _refreshTokenRegistry = refreshTokenRegistry;
        }
        
        [HttpDelete("{token}")]
        public async Task<IActionResult> Delete([FromRoute] string token)
        {
            await _refreshTokenRegistry.Remove(token);

            return Ok();
        }
    }
}