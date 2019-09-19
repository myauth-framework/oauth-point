using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IRefreshTokenRegistry _refreshTokenRegistry;

        /// <summary>
        /// Initializes a new instance of <see cref="SubjectsController"/>
        /// </summary>
        public SubjectsController(IRefreshTokenRegistry refreshTokenRegistry)
        {
            _refreshTokenRegistry = refreshTokenRegistry;
        }
        
        [HttpDelete("{subject}/refresh-tokens")]
        public async Task<IActionResult> DeleteRefreshTokens([FromRoute]string subject)
        {
            await _refreshTokenRegistry.RemoveBySubject(subject);
            return Ok();
        }
    }
}