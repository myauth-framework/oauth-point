using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Services
{
    class LoginService : ILoginService
    {
        private readonly IDbManager _db;
        private readonly IStringLocalizer<LoginService> _localizer;
        private readonly AuthOptions _opt;
        private readonly IDslLogger _log;

        public LoginService(IOptions<AuthOptions> opts, 
            IDbManager db, 
            ILogger<LoginService> log,
            IStringLocalizer<LoginService> localizer)
        {
            _db = db;
            _localizer = localizer;
            _opt = opts.Value;
            _log = log.Dsl();
        }

        public async Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest)
        {
            await using var dataContext = _db.Use();

            var clientAuthChecker = new ClientAuthorizationDbChecker(authorizationRequest.ClientId, dataContext, _localizer);

            await clientAuthChecker.CheckUser();
            await clientAuthChecker.CheckScopes(authorizationRequest.Scope);
            await clientAuthChecker.CheckRedirectUri(authorizationRequest.RedirectUri);
            
            var newSessionId = Guid.NewGuid().ToString("N");
            var newSessionExpiry = DateTime.Now.AddSeconds(_opt.LoginInitiationExpirySeconds);

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

        public async Task<LoginSession> GetActiveLoginSessionAsync(string loginSessionId)
        {
            var session = await GetLoginSessionAsync(loginSessionId);

            if (session.Expiry < DateTime.Now)
                throw new LoginSessionExpiredException()
                    .AndFactIs("login-session-id", loginSessionId);
            if (!session.IsSubjectAuthorized)
                throw new LoginSessionInvalidOperationException()
                    .AndFactIs("login-session-id", loginSessionId);

            return session;
        }

        public async Task SaveSuccessAsync(string loginSessionId, AuthorizedSubjectInfo authorizedSubjectInfo)
        {
            await using var dbConnection = _db.Use();

            var completer = new SessionDbInitiationCompleter(dbConnection, loginSessionId);

            var newExpiry = DateTime.Now.AddDays(_opt.LoginSessionExpiryDays);

            await completer.CompleteSuccessful(authorizedSubjectInfo, newExpiry);

            _log?.Warning("Login compete successfully")
                .AndFactIs("session-id", loginSessionId)
                .AndFactIs("subject", authorizedSubjectInfo.Subject)
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