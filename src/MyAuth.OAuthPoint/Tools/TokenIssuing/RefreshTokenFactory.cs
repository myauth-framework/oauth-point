using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class RefreshTokenFactory
    {
        private readonly string _tokenId;
        private readonly BaseClaimSet _claims;

        public RefreshTokenFactory(string tokenId, string sessionId, BaseClaimSet baseClaimSet)
        {
            _tokenId = tokenId;
            _claims = new BaseClaimSet
            {
                Expiry = baseClaimSet.Expiry,
                IssuedAt = baseClaimSet.IssuedAt,
                Issuer = baseClaimSet.Issuer,
                Audiences = new []{ baseClaimSet.Issuer },
                Subject = sessionId
            };
        }

        public string Create(string secret)
        {
            var claims = _claims.ToArray().ToList();
            claims.Add(new Claim("jti", _tokenId));

            JwtPayload payload = new JwtPayload();

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var header = new JwtHeader(new SigningCredentials(secKey, "HS256"));

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(t);
        }
    }
}