using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Db
{
    class SessionDbInitiationCompleter
    {
        private readonly DataConnection _dataConnection;
        private readonly string _loginSessionId;
        
        public SessionDbInitiationCompleter(DataConnection dataConnection, string loginSessionId)
        {
            _dataConnection = dataConnection;
            _loginSessionId = loginSessionId;
        }

        public async Task CompleteWithErrorAsync(LoginError loginError)
        {
            var updated = await _dataConnection.Tab<SessionInitiationDb>()
                .Where(s => s.SessionId == _loginSessionId)
                .Set(s => s.ErrorCode, s => loginError.AuthError)
                .Set(s => s.ErrorDesription, s => loginError.Description)
                .UpdateAsync();

            if(updated == 0)
                throw new LoginSessionNotFoundException(_loginSessionId);
        }

        public async Task CompleteSuccessful(AuthorizedUserInfo authorizedUserInfo, DateTime newExpiry)
        {
            var sessionInfo = await _dataConnection
                .Tab<SessionInitiationDb>()
                .Where(si => si.SessionId == _loginSessionId)
                .Select(si => new { Scope = si.Scope, Completed = si.CompleteDt != null })
                .FirstOrDefaultAsync();

            if (sessionInfo == null)
                throw new LoginSessionNotFoundException(_loginSessionId);

            if (sessionInfo.Completed)
                throw new LoginSessionInvalidOperationException(_loginSessionId);

            var requiredSessionScopes = sessionInfo.Scope;

            var scopes = new Dictionary<string, IDictionary<string, JObject>>();

            if (authorizedUserInfo.CustomScopes.Count != 0)
            {
                foreach (var customScope in authorizedUserInfo.CustomScopes)
                {
                    scopes.Add(customScope.Key, customScope.Value);
                }
            }

            if (authorizedUserInfo.Email != null)
                scopes.Add(StandardClaimsScopes.Email, authorizedUserInfo.Email.ToDictionary());
            if (authorizedUserInfo.Phone != null)
                scopes.Add(StandardClaimsScopes.Phone, authorizedUserInfo.Phone.ToDictionary());
            if (authorizedUserInfo.Address != null)
                scopes.Add(StandardClaimsScopes.Address, authorizedUserInfo.Address.ToDictionary());
            if (authorizedUserInfo.Profile != null)
                scopes.Add(StandardClaimsScopes.Profile, authorizedUserInfo.Profile.ToDictionary());

            await _dataConnection.PerformAutoTransactionAsync(async dc =>
            {
                await dc.BulkCopyAsync(
                    scopes.Select(kv =>
                        new SessionScopeDb
                        {
                            SessionId = _loginSessionId,
                            BoolRequired = requiredSessionScopes.Contains(kv.Key),
                            Name = kv.Key,
                            ClaimToSessionScopes = kv.Value.Select(ckv => new ClaimDb
                            {
                                Name = ckv.Key,
                                Value = ckv.Value.ToString().Trim('\"')
                            })
                        }));

                await dc.Tab<LoginSessionDb>()
                    .Where(s => s.Id == _loginSessionId)
                    .Set(s => s.Expiry, newExpiry)
                    .UpdateAsync();
            });

            
        }
    }
}