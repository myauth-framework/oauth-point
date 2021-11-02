using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class AccessTokenFactory
    {
        private readonly BaseClaimSet _baseClaimSet;
        private readonly ClaimsCollection _addClaims;
        public string Scope{ get; set; }

        public AccessTokenFactory(BaseClaimSet baseClaimSet, ClaimsCollection addClaims)
        {
            _baseClaimSet = baseClaimSet;
            _addClaims = addClaims;
        }

        public string Create(string secret)
        {
            if(secret == null)
                throw new ArgumentNullException(nameof(secret), "Symmetric key not defined");

            var claims = new List<Claim>();

            claims.AddRange(_baseClaimSet.ToArray());

            if (_addClaims != null)
            {
                claims.AddRange(_addClaims
                    .Select(c => new Claim(c.Key, c.Value.ToString())));
            }

            if (Scope != null)
                claims.Add(new Claim("scope", Scope));
            
            JwtPayload payload=  new JwtPayload(claims);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var header = new JwtHeader(new SigningCredentials(secKey, "HS256"));

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(t);
        }
    }
}
