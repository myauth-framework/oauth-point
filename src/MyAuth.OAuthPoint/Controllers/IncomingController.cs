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
using MyLab;
using MyLab.LogDsl;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IncomingController : ControllerBase
    {
        private readonly IRedisManager _redisManager;
        private readonly IClientRegistry _clientRegistry;

        public IncomingController(IRedisManager redisManager, IClientRegistry clientRegistry)
        {
            _redisManager = redisManager;
            _clientRegistry = clientRegistry;
        }
        
        // POST api/incoming
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest("Request id empty");
            if (string.IsNullOrWhiteSpace(loginRequest.ClientId))
                return BadRequest("ClientId is required");
            if (string.IsNullOrWhiteSpace(loginRequest.RedirectUri))
                return BadRequest("RedirectUri is required");
            if (string.IsNullOrWhiteSpace(loginRequest.UserId))
                return BadRequest("UserId is required");
            if (!string.IsNullOrEmpty(loginRequest.CodeChallenge) && 
                loginRequest.CodeChallengeMethod.ToLower() != "md5")
                return BadRequest("Code challenge method not supported");

            var client = _clientRegistry.GetClient(loginRequest.ClientId);
            if (client == null)
                return BadRequest("Client not found");
            if (client.Verification && string.IsNullOrEmpty(loginRequest.CodeChallenge))
                return BadRequest("Code challenge is required for this client");
            if (!CheckUri(loginRequest.RedirectUri, client))
                return BadRequest("Wrong redirect uri");
            
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
            
            bool CheckUri(string uri, ClientEntry c)
            {
                return c.AllowUris == null ||
                       c.AllowUris.Length == 0 ||
                       c.AllowUris.Any(u => u.ToLower() == uri.ToLower());
            }
        }
    }
}