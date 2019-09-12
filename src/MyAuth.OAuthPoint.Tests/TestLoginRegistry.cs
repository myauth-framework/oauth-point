using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tests
{
    internal class TestLoginRegistry : ILoginRegistry
    {
        public const string TestAuthCode = "34724f6039674735a2ab4c770b0e2192";
        public const string TestClientId = "0cec067f8dac4d189551202406e4147c";
        public const string TestUserId = "2bbddfc6a668492ebac555a28e7381e1";
        public const string TestRedirectUri = "http://test.host.ru/success";
        public const string TestCodeVerifier = "qwerty";
        public const string TestCodeChallenge = "2FeO34RYzgb7xbt2pYxcpA==";
        public const string TestRole = "Admin";
        public const string TestClimeName = "Clime";
        public const string TestClimeValue = "ClimeVal";
        
        private readonly IDictionary<string, LoginRequest> _requests
            = new Dictionary<string, LoginRequest>();

        public TestLoginRegistry()
        {
            _requests.Add(TestAuthCode, new LoginRequest
            {
                ClientId = TestClientId,
                CodeChallenge = TestCodeChallenge,
                RedirectUri = TestRedirectUri,
                UserId = TestUserId,
                CodeChallengeMethod = "MD5",
                Climes = new []{ new Clime{Name = TestClimeName, Value = TestClimeValue}  },
                Roles = new []{ TestRole}
            });
        }
        
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