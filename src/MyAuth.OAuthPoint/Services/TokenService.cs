using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Tools;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Services
{
    class TokenService : ITokenService
    {
        private readonly IDbManager _dbManager;
        private readonly IStringLocalizer<TokenService> _localizer;
        private readonly PasswordHashCalculator _passwordHashCalculator;
        private readonly AuthOptions _opt;

        public TokenService(IDbManager dbManager, 
            IStringLocalizer<TokenService> localizer,
            IOptions<AuthOptions> options)
        {
            _dbManager = dbManager;
            _localizer = localizer;
            _passwordHashCalculator = new PasswordHashCalculator(options.Value.ClientPasswordSalt);
            _opt = options.Value;
        }

        public async Task<SuccessfulTokenResponse> IssueAsync(string clientId, string clientPassword, TokenRequest request)
        {
            if (request.GrantType != "authorization_code")
                throw new TokenRequestProcessingException(_localizer["UnsupportedGrantType"], TokenRequestProcessingError.UnsupportedGrantType);

            await using var db = _dbManager.Use();

            var passwordHash = _passwordHashCalculator.CalcHexPasswordMd5(clientPassword);

            bool clientFound = await db.Tab<ClientDb>()
                .AnyAsync(c => c.Id == clientId && c.PasswordHash == passwordHash);

            if (!clientFound)
                throw new TokenRequestProcessingException(_localizer["ClientNotFound"], TokenRequestProcessingError.InvalidClient);

            var foundSession = await db.Tab<LoginSessionDb>()
                .Where(s => s.ClientId == clientId && s.AuthCode == request.Code)
                .Select(s => new
                {
                    s.RedirectUri,
                    s.Expiry,
                    s.AuthCodeUsed,
                    s.Scope,
                    s.SubjectId,
                    Audiences = s.Client.ClientAvailableAudienceToClients,
                    AccessClaims = s.Subject.SubjectAccessClaimsToSubjects,
                    IdentityClaims = s.Subject.SubjectIdentityClaimsToSubjects
                })
                .FirstOrDefaultAsync();

            if (foundSession == null)
                throw new TokenRequestProcessingException(_localizer["SessionNotFound"],
                    TokenRequestProcessingError.InvalidGrant);

            if (foundSession.Expiry <= DateTime.Now)
                throw new TokenRequestProcessingException(_localizer["SessionHasExpired"],
                    TokenRequestProcessingError.InvalidGrant);

            if (foundSession.AuthCodeUsed == MySqlBool.True)
                throw new TokenRequestProcessingException(_localizer["CodeUsed"],
                    TokenRequestProcessingError.InvalidGrant);

            if (foundSession.RedirectUri != request.RedirectUri)
                throw new TokenRequestProcessingException(_localizer["RedirectionUriMismatch"],
                    TokenRequestProcessingError.InvalidGrant);

            var accessTokenFactory = new AccessTokenFactory(_opt.TokenSecret)
            {
                Expiry = DateTime.Now.AddSeconds(_opt.AccessTokenExpirySeconds),
                Issuer = _opt.TokenIssuer,
                Subject = foundSession.SubjectId,
                Scope = foundSession.Scope,
                Audiences = foundSession.Audiences.Select(a => a.Uri).ToArray(),
                Claims = new ClaimsCollection(foundSession.AccessClaims.ToDictionary(c => c.Name, c => ClaimValue.Parse(c.Value)))
            };

            var accessToken = accessTokenFactory.Create();

            var requiredScopes = ScopeStringParser.Parse(foundSession.Scope);

            if (requiredScopes.Contains("openid"))
            {
                
            }

            return null;
        }
    }
}