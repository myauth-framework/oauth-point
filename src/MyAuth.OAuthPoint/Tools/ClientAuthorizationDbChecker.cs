using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Localization;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyLab.Db;
using MyLab.Log;

namespace MyAuth.OAuthPoint.Tools
{
    class ClientAuthorizationDbChecker
    {
        private readonly string _clientId;
        private readonly DataConnection _dc;
        private readonly IStringLocalizer _localizer;

        public ClientAuthorizationDbChecker(string clientId, DataConnection dc, IStringLocalizer localizer)
        {
            _clientId = clientId;
            _dc = dc;
            _localizer = localizer;
        }

        public async Task CheckUser()
        {
            bool found = await _dc.Tab<ClientDb>()
                .AnyAsync(c => c.Id == _clientId);

            if (!found)
            {
                throw new AuthorizationRequestProcessingException(
                        _localizer["ClientNotFound"],
                        AuthorizationRequestProcessingError.UnauthorizedClient)
                    .AndFactIs("client-id", _clientId);
            }
        }

        public async Task CheckScopes(string scopesString)
        {
            var scopes = scopesString
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            var allowed = await _dc.Tab<ClientScopeDb>()
                .Where(s => s.ClientId == _clientId && scopes.Contains(s.ScopeName))
                .Select(s => s.ScopeName)
                .ToArrayAsync();

            var disallowed = scopes
                .Where(s => !allowed.Contains(s))
                .ToArray();

            if (disallowed.Length != 0)
            {
                throw new AuthorizationRequestProcessingException(
                    _localizer["DisallowedScopes"],
                    AuthorizationRequestProcessingError.InvalidScope)
                    .AndFactIs("allowed_scopes", allowed)
                    .AndFactIs("disallowed_scopes", disallowed);
            }
        }

        public async Task CheckRedirectUri(string redirectUri)
        {
            bool found = await _dc.Tab<ClientRedirectUriDb>()
                .AnyAsync(u => u.ClientId == _clientId && u.Uri == redirectUri);

            if (!found)
            {
                throw new RedirectUriException(redirectUri, _localizer["DisallowedRedirectUri"])
                    .AndFactIs("redirect_uri", redirectUri);
            }
        }
    }
}
