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
    public interface ILoginSessionCreator
    {
        Task<string> CreateLoginSessionAsync(AuthorizationRequest authorizationRequest);
    }

    class LoginSessionCreator : ILoginSessionCreator
    {
        private readonly IDbManager _db;
        private readonly IStringLocalizer<LoginSessionCreator> _localizer;
        private readonly AuthTimingsOptions _opt;
        private readonly IDslLogger _log;

        public LoginSessionCreator(IOptions<AuthTimingsOptions> opts,
            IDbManager db,
            IStringLocalizer<LoginSessionCreator> localizer,
            ILogger<LoginSessionCreator> log = null)
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

            var loginSessionId = Guid.NewGuid().ToString("N");
            var tokenSessionId = Guid.NewGuid().ToString("N");
            var loginExpiry = DateTime.Now.AddSeconds(_opt.LoginExpirySeconds);
            var sessionExpiry = DateTime.Now.AddDays(_opt.SessionExpiryDays);

            await dc.PerformAutoTransactionAsync(async d =>
            {
                await dc.Tab<LoginSessionDb>()
                    .InsertAsync(() => new LoginSessionDb
                    {
                        Id = loginSessionId,
                        CreateDt = DateTime.Now,
                        Expiry = sessionExpiry,
                        LoginExpiry = loginExpiry
                    });

                await dc.Tab<TokenSessionDb>()
                    .InsertAsync(() => new TokenSessionDb()
                    {
                        Id = tokenSessionId,
                        LoginId = loginSessionId,
                        ClientId = authReq.ClientId,
                        RedirectUri = authReq.RedirectUri,
                        Scope = authReq.Scope,
                        State = authReq.State,
                        CreateDt = DateTime.Now
                    });
            });


            _log?.Action("Session created")
                .AndFactIs("request", authReq)
                .Write();

            return loginSessionId;

            
        }
    }
}
 