using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Services
{
    public interface ILoginSessionProvider
    {
        Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string loginId, string clientId, TokenSessionDbStatus expectedStatus);
    }

    class LoginSessionProvider : ILoginSessionProvider
    {
        private readonly IDbManager _db;

        public LoginSessionProvider(IDbManager db)
        {
            _db = db;
        }

        public async Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string loginId, string clientId, TokenSessionDbStatus expectedStatus)
        {
            await using var db = _db.Use();

            var q = db.Tab<TokenSessionDb>()
                .Where(s => 
                    s.LoginId == loginId && 
                    s.ClientId == clientId && 
                    s.Login.Expiry > DateTime.Now &&
                    (s.Status == expectedStatus || s.Status == TokenSessionDbStatus.Failed));
            
            return await q.Select(s => new SessionOAuth2Details
                {
                    LoginSessionId = s.LoginId,
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
