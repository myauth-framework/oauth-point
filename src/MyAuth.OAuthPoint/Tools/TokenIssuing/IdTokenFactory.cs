using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class IdTokenFactory
    {
        readonly List<Claim> _claims = new List<Claim>();

        public IdTokenFactory(BaseClaimSet baseClaimSet, ClaimsCollection userInfo)
        {
            _claims.AddRange(baseClaimSet.ToArray());
            _claims.AddRange(userInfo.Select(c => new Claim(c.Key, c.Value.ToString())));
        }

        public string Create(X509Certificate2 issuerCertificate)
        {
            JwtPayload payload = new JwtPayload(_claims);

            var signingCredentials = new X509SigningCredentials(issuerCertificate);

            var header = new JwtHeader(signingCredentials);

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(t);
        }
    }
}
