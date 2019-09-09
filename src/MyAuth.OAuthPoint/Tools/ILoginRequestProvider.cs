using System.Collections.Generic;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tools
{
    interface ILoginRequestProvider
    {
        Task<LoginRequest> Provide(string authCode);
    }
    
    public class LoginRequestProviderWithCache : ILoginRequestProvider
    {
        private readonly ILoginRegistry _loginRegistry;

        private readonly IDictionary<string, LoginRequest> _cache =
            new Dictionary<string, LoginRequest>();

        public LoginRequestProviderWithCache(ILoginRegistry loginRegistry)
        {
            _loginRegistry = loginRegistry;
        }

        public async Task<LoginRequest> Provide(string authCode)
        {
            if (_cache.TryGetValue(authCode, out var req))
                return req;

            req = await _loginRegistry.Get(authCode);
            _cache.Add(authCode, req);

            return req;
        }
    }
}