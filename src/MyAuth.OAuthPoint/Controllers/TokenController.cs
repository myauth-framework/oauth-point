using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotRedis.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IRedisManager _redisManager;

        public TokenController(IRedisManager redisManager)
        {
            _redisManager = redisManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TokenRequest tokenRequest)
        {
            if (tokenRequest == null)
                return BadRequest("Request id empty");
            if (tokenRequest.ClientId == null)
                return BadRequest("ClientId is required");
            if (tokenRequest.Code == null)
                return BadRequest("Authorization code is required");

            var keyName = "myauth:auth-code:" + tokenRequest.Code;
            
            LoginRequest loginRequest;
            
            using (var c = await _redisManager.GetConnection())
            {
                var requestKey = new RedisKey<LoginRequest>(keyName, c);

                if (!await requestKey.ExistsAsync())
                    return BadRequest("Authorization code not found");

                loginRequest = await requestKey.GetAsync();
            }

            if (loginRequest.ClientId != tokenRequest.ClientId)
                return BadRequest("Wrong ClientId");
            
            if (!string.IsNullOrEmpty(loginRequest.CodeChallenge) && string.IsNullOrEmpty(tokenRequest.CodeVerifier))
                return BadRequest("Wrong code verifier");

            if (!string.IsNullOrEmpty(tokenRequest.CodeVerifier) &&
                !CheckCodeVerifier(loginRequest.CodeChallenge, tokenRequest.CodeVerifier))
                return BadRequest("Wrong proof key");

            return Ok();

            bool CheckCodeVerifier(string loginRequestCodeChallenge, string tokenRequestCodeVerifier)
            {
                var binCodeVerifier = Convert.FromBase64String(tokenRequestCodeVerifier);
                
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(binCodeVerifier);

                var base64Hash = Convert.ToBase64String(hash);

                return loginRequestCodeChallenge == base64Hash;
            }
        }
    }
}