using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotRedis.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Models;
using MyLab;
using MyLab.LogDsl;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomingController : ControllerBase
    {
        private readonly IRedisManager _redisManager;

        public IncomingController(IRedisManager redisManager)
        {
            _redisManager = redisManager;
        }
        
        // POST api/incoming
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.ClientId))
                return BadRequest("ClientId is required");
            if (string.IsNullOrWhiteSpace(loginRequest.RedirectUri))
                return BadRequest("RedirectUri is required");
            if (string.IsNullOrWhiteSpace(loginRequest.UserId))
                return BadRequest("UserId is required");
            
            string authCode = Guid.NewGuid().ToString("N"); 
            
            var keyName = "myauth:auth-code:" + authCode;
            
            using (var c = await _redisManager.GetConnection())
            {
                var requestKey = new RedisKey<LoginRequest>(keyName, c);
                
                await requestKey.SetAsync(loginRequest);
                
#if DEBUG
                bool setExpResult = await requestKey.SetExpirationAsync(TimeSpan.FromSeconds(15));
#else
                bool setExpResult = await requestKey.SetExpirationAsync(TimeSpan.FromMinutes(1));
#endif
                if (!setExpResult)
                {
                    throw new InvalidOperationException("Can't set expiration")
                        .AndFactIs("request", loginRequest)
                        .AndFactIs("key", keyName);
                }
            }

            return Ok(authCode);
        }
    }
}