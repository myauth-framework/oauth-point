using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Services;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers.Api
{
    [Route("api/v1/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginSessionCompleter _loginSessionCompleter;

        /// <summary>
        /// Initializes a new instance of <see cref="LoginController"/>
        /// </summary>
        public LoginController(ILoginSessionCompleter loginSessionCompleter)
        {
            _loginSessionCompleter = loginSessionCompleter;
        }

        [HttpPost("{loginSessionId}/success")]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostSuccess([FromRoute]string loginSessionId, [FromBody] LoginSuccessRequest loginSuccessRequest)
        {
            if (string.IsNullOrWhiteSpace(loginSuccessRequest.Subject))
                return BadRequest("Subject not specified");

            await _loginSessionCompleter.CompleteSuccessfulAsync(loginSessionId, loginSuccessRequest);
            
            return Ok();
        }

        [HttpPost("{loginSessionId}/error")]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        public async Task<IActionResult> PostError([FromRoute] string loginSessionId, [FromBody] LoginErrorRequest loginErrorRequest)
        {
            await _loginSessionCompleter.CompleteErrorAsync(loginSessionId, loginErrorRequest);

            return Ok();
        }
    }
}
