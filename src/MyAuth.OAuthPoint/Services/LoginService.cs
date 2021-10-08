using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Services
{
    class LoginService : ILoginService
    {
        private readonly IDbManager _db;
        private readonly AuthOptions _opt;
        private readonly IDslLogger _log;

        public LoginService(IOptions<AuthOptions> opts, IDbManager db, ILogger<LoginService> log)
        {
            _db = db;
            _opt = opts.Value;
            _log = log.Dsl();
        }

        public async Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest)
        {
            var newSessionId = Guid.NewGuid().ToString("N");
            var newSessionExpiry = DateTime.Now.AddSeconds(_opt.LoginInitiationExpirySeconds);

            await using var dataContext = _db.Use();
            var creator = new SessionDbCreator(dataContext,newSessionId, newSessionExpiry, authorizationRequest);

            await creator.Create();

            return newSessionId;
        }

        public async Task<LoginSession> GetLoginSessionAsync(string loginSessionId)
        {
            await using var db = _db.Use();
            var provider = new SessionDbProvider(db, loginSessionId);

            return await provider.ProvideAsync();
        }

        public async Task SaveSuccessAsync(string loginSessionId, AuthorizedUserInfo authorizedUserInfo)
        {
            await using var dbConnection = _db.Use();

            var completer = new SessionDbInitiationCompleter(dbConnection, loginSessionId);

            var newExpiry = DateTime.Now.AddDays(_opt.LoginSessionExpiryDays);

            await completer.CompleteSuccessful(authorizedUserInfo, newExpiry);

            _log?.Warning("Login compete successfully")
                .AndFactIs("session-id", loginSessionId)
                .AndFactIs("subject", authorizedUserInfo.Subject)
                .Write();
        }

        public async Task SaveErrorAsync(string loginSessionId, LoginError loginError)
        {
            await using var dataConnection = _db.Use();

            var completer = new SessionDbInitiationCompleter(dataConnection, loginSessionId);
            
            await completer.CompleteWithErrorAsync(loginError);

            _log?.Warning("Login error")
                .AndFactIs("error", loginError)
                .AndFactIs("session-id", loginSessionId)
                .Write();
        }
    }
}