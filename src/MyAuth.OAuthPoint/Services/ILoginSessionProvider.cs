﻿using System;
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
        Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string loginId, string clientId,bool shouldLogon);
    }

    class LoginSessionProvider : ILoginSessionProvider
    {
        private readonly IDbManager _db;

        public LoginSessionProvider(IDbManager db)
        {
            _db = db;
        }

        public async Task<SessionOAuth2Details> ProvideOAuth2DetailsAsync(string loginId, string clientId, bool shouldLogon)
        {
            await using var db = _db.Use();

            var q = db.Tab<TokenSessionDb>()
                .Where(s => 
                    s.LoginId == loginId && 
                    s.ClientId == clientId && 
                    s.Login.Expiry > DateTime.Now);

            q = shouldLogon 
                ? q.Where(s => s.Status == TokenSessionDbStatus.Started) 
                : q.Where(s => s.Status == TokenSessionDbStatus.Pending);

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