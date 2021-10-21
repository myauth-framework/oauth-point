using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    class AccessTokenFactory
    {
        private readonly string _secret;
        public string Subject { get; set; }
        public DateTime Expiry { get; set; }
        public ClaimsCollection Claims { get; set; }
        public string[] Audiences { get; set; }
        public string Issuer { get; set; }
        public string Scope{ get; set; }

        public AccessTokenFactory(string secret)
        {
            _secret = secret;
        }

        public string Create()
        {
            var clms = new List<Claim> {new Claim("sub", Subject)};

            if (Claims != null)
            {
                clms.AddRange(Claims
                    .Select(c => new Claim(c.Key, c.Value.ToString().Trim('\"'))));
            }

            if (Scope != null)
            {
                clms.Add(new Claim("scope", Scope));
            }

            if (Audiences != null)
            {
                foreach (var audience in Audiences)
                {
                    clms.Add(new Claim("aud", audience));
                }
            }

            JwtPayload payload=  new JwtPayload(Issuer, null, clms, DateTime.Now, Expiry);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var header = new JwtHeader(new SigningCredentials(secKey, "HS256"));

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(t);
        }
    }
}
