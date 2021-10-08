using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Db
{
    class SessionDbProvider
    {
        private readonly DataConnection _dataConnection;
        private readonly string _loginSessionId;

        public SessionDbProvider(DataConnection dataConnection, string loginSessionId)
        {
            _dataConnection = dataConnection;
            _loginSessionId = loginSessionId;
        }

        public async Task<LoginSession> ProvideAsync()
        {
            var sessionInfo = await LoadSessionInfoAsync(_dataConnection, _loginSessionId);

            if (sessionInfo != null)
            {
                if (sessionInfo.IsUserAuthorized)
                {

                    var claims = await LoadClaimsAsync(_loginSessionId, _dataConnection);

                    var addressScope = TryCreateScope<AddressScopeClaims>(claims, StandardClaimsScopes.Address);
                    var phoneScope = TryCreateScope<PhoneScopeClaims>(claims, StandardClaimsScopes.Phone);
                    var emailScope = TryCreateScope<EmailScopeClaims>(claims, StandardClaimsScopes.Email);
                    var profileScope = TryCreateScope<ProfileScopeClaims>(claims, StandardClaimsScopes.Profile);

                    var customScopes = SelectCustomScopes(claims);

                    sessionInfo.Session.AuthorizedUserInfo = new AuthorizedUserInfo
                    {
                        Subject = sessionInfo.Subject,
                        Address = addressScope,
                        Email = emailScope,
                        Phone = phoneScope,
                        Profile = profileScope
                    };

                    if (customScopes.Count != 0)
                    {
                        sessionInfo.Session.AuthorizedUserInfo.CustomScopes = customScopes;
                    }
                }
                else
                {
                    throw new LoginSessionInvalidOperationException(_loginSessionId);
                }
            }
            else
            {
                throw new LoginSessionNotFoundException(_loginSessionId);
            }

            return sessionInfo.Session;
        }

        private static Dictionary<string, CustomScopeClaims> SelectCustomScopes(Tuple<string, string, string>[] claims)
        {
            return claims
                .Where(c => !StandardClaimsScopes.Scopes.Contains(c.Item1))
                .GroupBy(c => c.Item1)
                .ToDictionary(
                    g => g.Key,
                    g => new CustomScopeClaims(
                        g.ToDictionary(
                            t => t.Item2,
                            t => JsonClaimValueParser.Parse(t.Item3)
                        ))
                );
        }

        private static async Task<Tuple<string, string, string>[]> LoadClaimsAsync(string loginSessionId, DataConnection dbConnection)
        {
            return await dbConnection.Tab<ClaimDb>()
                .Where(c =>
                    c.ClaimToSessionScope.SessionId == loginSessionId &&
                    c.ClaimToSessionScope.Required == 'Y')
                .Select(c => new Tuple<string, string, string>
                (
                    c.ClaimToSessionScope.Name,
                    c.Name,
                    c.Value
                ))
                .ToArrayAsync();
        }

        Task<SessionInfo> LoadSessionInfoAsync(DataConnection dbConnection, string loginSessionId)
        {
            return dbConnection.Tab<LoginSessionDb>()
                .Where(s => s.Id == loginSessionId && s.Expiry > DateTime.Now)
                .Select(s => new SessionInfo
                {
                    Session = new LoginSession
                    {
                        Id = s.Id,
                        Expiry = s.Expiry,
                        ClientId = s.ClientId,
                        InitDetails = new LoginInitDetails
                        {
                            RedirectUri = s.InitiationToSession.RedirectUri,
                            State = s.InitiationToSession.State,
                            AuthorizationCode = s.InitiationToSession.AuthorizationCode,
                            Error = s.InitiationToSession.ErrorCode != AuthorizationRequestProcessingError.Undefined
                                ? null
                                : new LoginSessionError
                                {
                                    Error = s.InitiationToSession.ErrorCode,
                                    Description = s.InitiationToSession.ErrorDesription
                                }
                        }
                    },
                    IsUserAuthorized = s.LoginDt != null,
                    Subject = s.Subject
                })
                .FirstOrDefaultAsync();
        }

        T TryCreateScope<T>(IEnumerable<Tuple<string, string, string>> claims, string scopeName)
            where T : ScopeClaims, new()
        {
            var scopeClaims = claims
                .Where(c => c.Item1 == scopeName)
                .ToDictionary(
                    c => c.Item2,
                    c => JsonClaimValueParser.Parse(c.Item3));

            if (scopeClaims.Count == 0) return null;

            var scope = new T();

            scope.Load(scopeClaims);

            return scope;
        }

        class SessionInfo
        {
            public LoginSession Session { get; set; }
            public bool IsUserAuthorized { get; set; }
            public string Subject { get; set; }
        }
    }
}
