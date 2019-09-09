using System;
using System.Threading.Tasks;
using DotRedis.ObjectModel;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using MyLab;
using MyLab.RedisManager;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginRegistry
    {
        Task Register(string authCode, LoginRequest request);
        Task<LoginRequest> Get(string authCode);
    }
    
    public class DefaultLoginRegistry : ILoginRegistry
    {
        private readonly IRedisManager _redisManager;

        public DefaultLoginRegistry(IRedisManager redisManager)
        {
            _redisManager = redisManager;
        }
        
        public async Task Register(string authCode, LoginRequest request)
        {
            var keyName = LoginRedisKey.Create(authCode);
            
            using (var c = await _redisManager.GetConnection())
            {
                var requestKey = new RedisKey<LoginRequest>(keyName, c);
                
                await requestKey.SetAsync(request);
                
#if DEBUG
                bool setExpResult = await requestKey.SetExpirationAsync(TimeSpan.FromSeconds(15));
#else
                bool setExpResult = await requestKey.SetExpirationAsync(TimeSpan.FromMinutes(1));
#endif
                if (!setExpResult)
                {
                    throw new InvalidOperationException("Can't set expiration")
                        .AndFactIs("request", request)
                        .AndFactIs("key", keyName);
                }
            }
        }

        public async Task<LoginRequest> Get(string authCode)
        {
            var keyName = LoginRedisKey.Create(authCode);
            
            using (var c = await _redisManager.GetConnection())
            {
                var requestKey = new RedisKey<LoginRequest>(keyName, c);
                return await requestKey.GetAsync();
            }
        }
    }
}