using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyAuth.OAuthPoint.Services
{
    public interface ISessionInitiator
    {
        Task CompleteSuccessfulAsync(string loginSessionId, LoginSuccessRequest loginSuccessRequest);

        Task CompleteErrorAsync(string loginSessionId, LoginErrorRequest loginErrorRequest);
    }

    class SessionInitiator : ISessionInitiator
    {
        private readonly IDbManager _db;
        private readonly IDslLogger _log;

        public SessionInitiator(IDbManager db,
            ILogger<SessionInitiator> log = null)
        {
            _db = db;
            _log = log?.Dsl();
        }

        public async Task CompleteSuccessfulAsync(string sessId, LoginSuccessRequest succReq)
        {
            await using var db = _db.Use();

            var sessionFound = await db
                .Tab<LoginSessionDb>()
                .AnyAsync(s => s.Id == sessId && s.Completed != MySqlBool.True && s.LoginExpiry > DateTime.Now);

            if (!sessionFound)
                throw new LoginSessionNotFoundException()
                    .AndFactIs("login-session-id", sessId);

            await db.PerformAutoTransactionAsync(async d =>
            {
                var logic = new SuccessfulLoginApplyLogic(sessId, d, succReq);

                await logic.CreateSubjectIfNotExistsAsync();
                await logic.RemoveSubjectIdentityClaimsAsync();
                await logic.SaveIdentityClaimsAsync();
                await logic.RemoveSubjectAccessClaimsAsync();
                await logic.SaveAccessClaimsAsync();
                await logic.UpdateSessionStateAsync();
            });
        }

        public async Task CompleteErrorAsync(string sessId, LoginErrorRequest errReq)
        {
            await using var db = _db.Use();
            
            var updated = await db.Tab<LoginSessionDb>()
                .Where(s => s.Id == sessId && s.Completed != MySqlBool.True && s.LoginExpiry > DateTime.Now)
                .Set(s => s.ErrorCode, s => errReq.AuthError)
                .Set(s => s.ErrorDesc, s => errReq.Description)
                .Set(s => s.Completed, s => MySqlBool.True)
                .Set(s => s.CompletedDt, s => DateTime.Now)
                .UpdateAsync();
            
            if(updated == 0)
                throw new LoginSessionNotFoundException()
                    .AndFactIs("session-id", sessId);

            _log?.Warning("Login error")
                .AndFactIs("error", errReq)
                .AndFactIs("session-id", sessId)
                .Write();
        }
    }
}
