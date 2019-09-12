using System.Collections.Generic;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tests
{
    class TestRefreshTokenRegistry : IRefreshTokenRegistry
    {
        readonly IDictionary<string, LoginRequest> _loginRequests =
            new Dictionary<string, LoginRequest>();

        readonly IDictionary<string, List<string>> _subjectTokenList =
            new Dictionary<string, List<string>>();
        
        public Task Register(string token, RefreshTokenDescription desc)
        {
            _loginRequests.Add(token, desc.LoginRequest);

            if (!_subjectTokenList.TryGetValue(desc.LoginRequest.Subject, out var l))
            {
                l = new List<string>();
                _subjectTokenList.Add(desc.LoginRequest.Subject, l);
            }

            l.Add(token);

            return Task.CompletedTask;
        }

        public Task<LoginRequest> Get(string token)
        {
            return Task.FromResult(_loginRequests.TryGetValue(token, out var t) ? t : null);
        }

        public Task Remove(string token)
        {
            return Task.FromResult(_loginRequests.Remove(token));
        }

        public Task RemoveBySubject(string subject)
        {
            var list = _subjectTokenList[subject];
            foreach (var token in list)
            {
                _loginRequests.Remove(token);
            }

            _subjectTokenList.Remove(subject);

            return Task.CompletedTask;
        }
    }
}