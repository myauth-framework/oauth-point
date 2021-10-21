using System;
using System.Linq;
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

        public TokenService(IDbManager dbManager, 
            IStringLocalizer<TokenService> localizer,
            IOptions<AuthOptions> options)
        {
            _dbManager = dbManager;
            _localizer = localizer;
            _passwordHashCalculator = new PasswordHashCalculator(options.Value.ClientPasswordSalt);
        }

        public async Task<SuccessfulTokenResponse> IssueAsync(string clientId, string clientPassword, TokenRequest request)
        {
            return null;
            //if(request.GrantType != "authorization_code")
            //    throw new TokenRequestProcessingException(_localizer["UnsupportedGrantType"], TokenRequestProcessingError.UnsupportedGrantType);

            //await using var db = _dbManager.Use();

            //var passwordHash = _passwordHashCalculator.CalcHexPasswordMd5(clientPassword);

            //bool clientFound = await db.Tab<ClientDb>()
            //    .AnyAsync(c => c.Id == clientId && c.PassMd5 == passwordHash);

            //if(!clientFound)
            //    throw new TokenRequestProcessingException(_localizer["ClientNotFound"], TokenRequestProcessingError.InvalidClient);

            //var foundSession = await db.Tab<SessionInitiationDb>()
            //    .Where(s => s.Session.ClientId == clientId && s.AuthorizationCode == request.Code)
            //    .Select(s => new
            //    { 
            //        s.RedirectUri,
            //        s.Session.Expiry,
            //        s.BoolCodeUsed
            //    })
            //    .FirstOrDefaultAsync();

            //if (foundSession == null)
            //    throw new TokenRequestProcessingException(_localizer["SessionNotFound"],
            //        TokenRequestProcessingError.InvalidGrant);

            //if (foundSession.Expiry <= DateTime.Now)
            //    throw new TokenRequestProcessingException(_localizer["SessionHasExpired"],
            //        TokenRequestProcessingError.InvalidGrant);

            //if (foundSession.BoolCodeUsed)
            //    throw new TokenRequestProcessingException(_localizer["CodeUsed"],
            //        TokenRequestProcessingError.InvalidGrant);

            //if (foundSession.RedirectUri != request.RedirectUri)
            //    throw new TokenRequestProcessingException(_localizer["RedirectionUriMismatch"],
            //        TokenRequestProcessingError.InvalidGrant);
        }
    }
}