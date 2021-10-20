using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyLab.Log.Dsl;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Controllers.Api
{
    [Route("api/v1/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IDslLogger _log;

        /// <summary>
        /// Initializes a new instance of <see cref="LoginController"/>
        /// </summary>
        public LoginController(ILoginService loginService,
            ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _log = logger.Dsl();
        }

        [HttpPost("{loginSessionId}/success")]
        [ErrorToResponse(typeof(LoginSessionExpiredException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionInvalidOperationException), HttpStatusCode.Conflict)]
        public async Task<IActionResult> PostSuccess([FromRoute]string loginSessionId, [FromBody]AuthorizedSubjectInfo authorizedSubjectInfo)
        {
            await _loginService.SaveSuccessAsync(loginSessionId, authorizedSubjectInfo);
            
            return Ok();
        }

        [HttpPost("{loginSessionId}/error")]
        [ErrorToResponse(typeof(LoginSessionExpiredException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionNotFoundException), HttpStatusCode.NotFound)]
        [ErrorToResponse(typeof(LoginSessionInvalidOperationException), HttpStatusCode.Conflict)]
        public async Task<IActionResult> PostError([FromRoute] string loginSessionId, [FromBody] LoginError loginError)
        {
            await _loginService.SaveErrorAsync(loginSessionId, loginError);

            return Ok();
        }
    }
}
