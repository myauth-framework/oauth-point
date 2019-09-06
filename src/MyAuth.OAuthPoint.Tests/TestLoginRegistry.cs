using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tests
{
    internal class TestLoginRegistry : ILoginRegistry
    {
        private readonly IDictionary<string, LoginRequest> _requests
            = new Dictionary<string, LoginRequest>();
        public Task Register(string authCode, LoginRequest request)
        {
            _requests.TryAdd(authCode, request);

            return Task.CompletedTask;
        }

        public Task<LoginRequest> Get(string authCode)
        {
            return Task.FromResult(_requests.TryGetValue(authCode, out var res)
                ? res
                : null);
        }
    }
}