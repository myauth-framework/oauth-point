using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotRedis.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyAuth.OAuthPoint.Tools;
using MyLab;
using MyLab.LogDsl;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRegistry _loginRegistry;
        private readonly LoginRequestChecker _requestChecker;

        public LoginController(ILoginRegistry loginRegistry, IClientRegistry clientRegistry)
        {
            _loginRegistry = loginRegistry;
            _requestChecker = new LoginRequestChecker(clientRegistry);
        }
        
        // POST api/incoming
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
        {
            if (!_requestChecker.Check(loginRequest, out string errText))
                return BadRequest(errText);
            
            string authCode = Guid.NewGuid().ToString("N");

            _loginRegistry.Register(authCode, loginRequest);

            return Ok(authCode);
            
            
        }
    }
}