using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginSessionCompleter
    {
        Task CompleteSuccessfulAsync(string loginSessId, LoginSuccessRequest loginSuccessRequest);

        Task CompleteErrorAsync(string loginSessId, LoginErrorRequest loginErrorRequest);
    }

    class LoginSessionCompleter : ILoginSessionCompleter
    {
        private readonly IDbManager _db;
        private readonly IDslLogger _log;
        private readonly AuthTimingsOptions _opts;

        public LoginSessionCompleter(IDbManager db,
            IOptions<AuthTimingsOptions> timingOpts,
            ILogger<LoginSessionCompleter> log = null)
        {
            _db = db;
            _log = log?.Dsl();
            _opts = timingOpts.Value;
        }

        public async Task CompleteSuccessfulAsync(string loginSessId, LoginSuccessRequest succReq)
        {
            await using var db = _db.Use();

            var sessionFound = await db
                .Tab<LoginSessionDb>()
                .AnyAsync(s => 
                    s.Id == loginSessId && 
                    s.Status == LoginSessionDbStatus.Pending && 
                    s.LoginExpiry > DateTime.Now);

            if (!sessionFound)
                throw new LoginSessionNotFoundException()
                    .AndFactIs("login-session-id", loginSessId);

            var authCode = Guid.NewGuid().ToString("N");
            var authCodeExpiry = DateTime.Now.AddSeconds(_opts.AuthCodeExpirySeconds);

            await db.PerformAutoTransactionAsync(async d =>
            {
                var logic = new SuccessfulLoginApplyLogic(loginSessId, d, succReq);

                await logic.CreateSubjectIfNotExistsAsync();
                await logic.RemoveSubjectIdentityClaimsAsync();
                await logic.SaveIdentityClaimsAsync();
                await logic.RemoveSubjectAccessClaimsAsync();
                await logic.SaveAccessClaimsAsync();
                await logic.UpdateSessionStateAsync(authCode, authCodeExpiry);
            });
        }

        public async Task CompleteErrorAsync(string loginSessId, LoginErrorRequest errReq)
        {
            await using var db = _db.Use();

            int updatedSessions = 0;

            await db.PerformAutoTransactionAsync(async d =>
            {
                updatedSessions = await db.Tab<LoginSessionDb>()
                    .Where(s =>
                        s.Id == loginSessId &&
                        s.Status == LoginSessionDbStatus.Pending &&
                        s.LoginExpiry > DateTime.Now)
                    .Set(s => s.Status, s => LoginSessionDbStatus.Failed)
                    .UpdateAsync();

                await db.Tab<TokenSessionDb>()
                    .Where(s =>
                        s.LoginId == loginSessId &&
                        s.Status == TokenSessionDbStatus.Pending)
                    .Set(s => s.Status, s => TokenSessionDbStatus.Failed)
                    .Set(s => s.ErrorCode, s => errReq.AuthError)
                    .Set(s => s.ErrorDesc, s => errReq.Description)
                    .UpdateAsync();
            });

            if(updatedSessions == 0)
                throw new LoginSessionNotFoundException()
                    .AndFactIs("session-id", loginSessId);

            _log?.Warning("Login error")
                .AndFactIs("error", errReq)
                .AndFactIs("session-id", loginSessId)
                .Write();
        }
    }
}
