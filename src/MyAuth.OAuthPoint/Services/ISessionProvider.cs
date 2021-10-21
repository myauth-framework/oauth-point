using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Services
{
    public interface ISessionProvider
    {
        Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string sessionId, bool successCompletionRequired = false);
    }

    class SessionProvider : ISessionProvider
    {
        private readonly IDbManager _db;

        public SessionProvider(IDbManager db)
        {
            _db = db;
        }

        public async Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string sessionId, bool successCompletionRequired)
        {
            await using var db = _db.Use();

            var q = db.Tab<LoginSessionDb>()
                .Where(s => s.Expiry > DateTime.Now);

            if (successCompletionRequired)
                q = q.Where(s => 
                    s.Completed == MySqlBool.True &&
                    s.ErrorCode == AuthorizationRequestProcessingError.Undefined);

            return await q.Select(s => new SessionOAuth2Details
                {
                    RedirectUri = s.RedirectUri,
                    Scope = s.Scope,
                    State = s.State,
                    AuthorizationCode = s.AuthCode,
                    ClientId = s.ClientId,
                    ErrorCode = s.ErrorCode,
                    ErrorDescription = s.ErrorDesc
                })
                .FirstOrDefaultAsync();
        }
    }
}
