using System;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Services
{
    public interface ISessionCreator
    {
        Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest);
    }

    class SessionCreator : ISessionCreator
    {
        private readonly IDbManager _db;
        private readonly IStringLocalizer<SessionCreator> _localizer;
        private readonly AuthTimingsOptions _opt;
        private readonly IDslLogger _log;

        public SessionCreator(IOptions<AuthTimingsOptions> opts,
            IDbManager db,
            IStringLocalizer<SessionCreator> localizer,
            ILogger<SessionCreator> log = null)
        {
            _db = db;
            _localizer = localizer;
            _opt = opts.Value;
            _log = log?.Dsl();
        }

        public async Task<string> CreateLoginSessionAsync(AuthorizationRequest authReq)
        {
            await using var dc = _db.Use();

            var clientAuthChecker = new ClientAuthorizationDbChecker(authReq.ClientId, dc, _localizer);

            await clientAuthChecker.CheckUser();
            await clientAuthChecker.CheckScopes(authReq.Scope);
            await clientAuthChecker.CheckRedirectUri(authReq.RedirectUri);

            var newSessionId = Guid.NewGuid().ToString("N");
            var loginExpiry = DateTime.Now.AddSeconds(_opt.LoginExpirySeconds);
            var sessionExpiry = DateTime.Now.AddDays(_opt.SessionExpiryDays);

            await dc.Tab<LoginSessionDb>()
                .InsertAsync(() => new LoginSessionDb
                {
                    Id = newSessionId,
                    ClientId = authReq.ClientId,
                    RedirectUri = authReq.RedirectUri,
                    Scope = authReq.Scope,
                    State = authReq.State,
                    CreateDt = DateTime.Now,
                    Expiry = sessionExpiry,
                    LoginExpiry = loginExpiry
                });

            _log?.Action("Session created")
                .AndFactIs("request", authReq)
                .Write();

            return newSessionId;

            
        }
    }
}
 