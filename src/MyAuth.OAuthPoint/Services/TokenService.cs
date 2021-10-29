using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Models.DataContract;
using MyAuth.OAuthPoint.Tools;
using MyAuth.OAuthPoint.Tools.TokenIssuing;
using MyLab.Db;

namespace MyAuth.OAuthPoint.Services
{
    class TokenService : ITokenService
    {
        private readonly IDbManager _dbManager;
        private readonly IStringLocalizer<TokenService> _localizer;
        private readonly PasswordHashCalculator _passwordHashCalculator;
        private readonly IX509CertificateProvider _x509CertificateProvider;
        private readonly TokenIssuingOptions _tokenIssuingOptions;

        public TokenService(IDbManager dbManager, 
            IStringLocalizer<TokenService> localizer,
            PasswordHashCalculator passwordHashCalculator,
            IX509CertificateProvider x509CertificateProvider,
            IOptions<TokenIssuingOptions> tokenIssuingOptions)
        {
            _dbManager = dbManager;
            _localizer = localizer;
            _passwordHashCalculator = passwordHashCalculator;
            _x509CertificateProvider = x509CertificateProvider;
            _tokenIssuingOptions = tokenIssuingOptions.Value;
        }

        public async Task<SuccessfulTokenResponse> IssueAsync(string clientId, string clientPassword, TokenRequest request)
        {
            VerifyRequestProperties(clientId, request);

            await using var db = _dbManager.Use();

            await VerifyClientAsync(clientId, clientPassword, db);

            switch (request.GrantType)
            {
                case "authorization_code":
                    return await IssueNew(db, clientId, request);
                case "refresh_token":
                    return await RefreshToken(db, clientId, request);
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.GrantType));
            }
        }

        private async Task<SuccessfulTokenResponse> RefreshToken(DataConnection db, string clientId, TokenRequest request)
        {
            var foundSession = await db.Tab<TokenSessionDb>()
                .Where(s => s.ClientId == clientId && s.Id == request.RefreshToken)
                .Select(_storedSessionInfoSelector)
                .FirstOrDefaultAsync();

            VerifySessionAsync(foundSession, null);

            return IssueTokens(foundSession);
        }

        private async Task<SuccessfulTokenResponse> IssueNew(DataConnection db, string clientId, TokenRequest request)
        {
            var foundSession = await db.Tab<TokenSessionDb>()
                .Where(s => s.ClientId == clientId && s.AuthCode == request.Code)
                .Select(_storedSessionInfoSelector)
                .FirstOrDefaultAsync();

            VerifySessionAsync(foundSession, request.RedirectUri);

            var response = IssueTokens(foundSession);

            response.RefreshToken = foundSession.Id;

            return response;
        }

        private SuccessfulTokenResponse IssueTokens(StoredSessionInfo foundSession)
        {
            var issueDt = DateTime.Now;
            var accessTokenExpiration = issueDt.AddSeconds(_tokenIssuingOptions.AccessTokenExpirySeconds);

            var response = new SuccessfulTokenResponse
            {
                TokenType = "Bearer",
                ExpiresIn = EpochTime.GetIntDate(accessTokenExpiration),
                Scope = foundSession.Scope
            };

            var baseClaims = new BaseClaimSet
            {
                Audiences = foundSession.Audiences.Select(a => a.Uri).ToArray(),
                Issuer = _tokenIssuingOptions.Issuer,
                IssuedAt = issueDt,
                Subject = foundSession.SubjectId
            };

            response.AccessToken = IssueAccessToken(foundSession.Scope, baseClaims, accessTokenExpiration, foundSession);

            var scopes = ScopeStringParser.Parse(foundSession.Scope);

            if (scopes.Contains("openid"))
            {
                response.IdToken = IssueIdToken(foundSession, scopes, baseClaims);
            }

            return response;
        }

        void VerifyRequestProperties(string credentialsClientId, TokenRequest request)
        {
            switch (request.GrantType)
            {
                case "authorization_code":
                {
                    if (string.IsNullOrWhiteSpace(request.Code))
                        throw new TokenRequestProcessingException(_localizer["NoAuthCode"],
                            TokenRequestProcessingError.InvalidRequest);

                    if (string.IsNullOrWhiteSpace(request.RedirectUri))
                        throw new TokenRequestProcessingException(_localizer["NoRedirectionUri"],
                            TokenRequestProcessingError.InvalidRequest);

                    if (!Uri.IsWellFormedUriString(request.RedirectUri, UriKind.Absolute))
                        throw new TokenRequestProcessingException(_localizer["NoRedirectionMalformed"],
                            TokenRequestProcessingError.InvalidRequest);
                }
                    break;
                case "refresh_token":
                {
                    if (string.IsNullOrWhiteSpace(request.RefreshToken))
                        throw new TokenRequestProcessingException(_localizer["NoRefreshToken"],
                            TokenRequestProcessingError.InvalidRequest);
                }
                    break;
                default:
                    throw new TokenRequestProcessingException(_localizer["UnsupportedGrantType"],
                        TokenRequestProcessingError.UnsupportedGrantType);
            }

            if (credentialsClientId != request.ClientId)
            {
                throw new TokenRequestProcessingException(_localizer["RequestClientMismatch"],
                    TokenRequestProcessingError.InvalidRequest);
            }
        }
        private string IssueIdToken(StoredSessionInfo foundSession, string[] scopes, BaseClaimSet baseClaims)
        {
            var userInfoClaims = new ClaimsCollection(
                foundSession.IdentityClaims
                    .Where(ic => scopes.Contains(ic.ScopeId))
                    .ToDictionary(
                        ci => ci.Name,
                        ci => ClaimValue.Parse(ci.Value)
                    )
            );
            var baseClaimsForIdToken = baseClaims.WithExpiry(foundSession.Expiry);

            var idTokenFactory = new IdTokenFactory(baseClaimsForIdToken, userInfoClaims);

            var issuerCertificate = _x509CertificateProvider.ProvideIssuerCertificate();

            return idTokenFactory.Create(issuerCertificate);
        }

        private string IssueAccessToken(string scope, BaseClaimSet baseClaims, DateTime accessTokenExpiration, StoredSessionInfo foundSession)
        {
            var baseClaimsForAccessToken = baseClaims.WithExpiry(accessTokenExpiration);

            var addAccessClaims = new ClaimsCollection(foundSession.AccessClaims
                .ToDictionary(
                    c => c.Name,
                    c => ClaimValue.Parse(c.Value)
                ));

            var accessTokenFactory = new AccessTokenFactory(baseClaimsForAccessToken, addAccessClaims)
            {
                Scope = scope
            };

            return accessTokenFactory.Create(_tokenIssuingOptions.SignSymmetricKey);
        }

        private readonly Expression<Func<TokenSessionDb, StoredSessionInfo>> _storedSessionInfoSelector = s =>
            new StoredSessionInfo
            {
                Id = s.Id,
                RedirectUri = s.RedirectUri,
                Expiry = s.Login.Expiry,
                AuthCodeDisabled = s.Status != TokenSessionDbStatus.Pending,
                Scope = s.Scope,
                SubjectId = s.Login.SubjectId,
                Audiences = s.Client.ClientAvailableAudienceToClients.ToArray(),
                AccessClaims = s.Login.Subject.SubjectAccessClaimsToSubjects.ToArray(),
                IdentityClaims = s.Login.Subject.SubjectIdentityClaimsToSubjects.ToArray()
            };

        private void VerifySessionAsync(StoredSessionInfo foundSession, string verifyRequestRedirectUri)
        {
            if (foundSession == null)
                throw new TokenRequestProcessingException(_localizer["SessionNotFound"],
                    TokenRequestProcessingError.InvalidGrant);
            
            if (foundSession.Revoked)
                throw new TokenRequestProcessingException(_localizer["RevokedSession"],
                    TokenRequestProcessingError.InvalidGrant);

            if (foundSession.Expiry <= DateTime.Now)
                throw new TokenRequestProcessingException(_localizer["SessionHasExpired"],
                    TokenRequestProcessingError.InvalidGrant);

            if (foundSession.AuthCodeDisabled)
                throw new TokenRequestProcessingException(_localizer["CodeUsed"],
                    TokenRequestProcessingError.InvalidGrant);

            if (verifyRequestRedirectUri != null && foundSession.RedirectUri != verifyRequestRedirectUri)
                throw new TokenRequestProcessingException(_localizer["RedirectionUriMismatch"],
                    TokenRequestProcessingError.InvalidGrant);
        }

        private async Task VerifyClientAsync(string clientId, string clientPassword, DataConnection db)
        {
            var passwordHash = _passwordHashCalculator.CalcHexPasswordMd5(clientPassword);

            bool clientFound = await db.Tab<ClientDb>()
                .AnyAsync(c => c.Id == clientId && c.PasswordHash == passwordHash);

            if (!clientFound)
                throw new TokenRequestProcessingException(_localizer["ClientNotFound"],
                    TokenRequestProcessingError.InvalidClient);
        }

        class StoredSessionInfo
        { 
            public string Id { get; set; }
            public string RedirectUri { get; set; }
            public DateTime Expiry { get; set; }
            public bool AuthCodeDisabled { get; set; }
            public bool Revoked { get; set; }
            public string Scope { get; set; }
            public string SubjectId { get; set; }
            public ClientAvailableAudienceDb[] Audiences { get; set; }
            public SubjectAccessClaimDb[] AccessClaims { get; set; }
            public SubjectIdentityClaimDb[] IdentityClaims { get; set; }
        }
    }
}