using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class BaseClaimSet
    {
        public string Subject { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime? IssuedAt { get; set; }
        public string Issuer { get; set; }
        public string[] Audiences { get; set; }

        public Claim[] ToArray()
        {
            var claims = new List<Claim>();

            if(Subject != null) claims.Add(new Claim("sub", Subject));
            
            if (Expiry.HasValue)
            {
                var expDt = EpochTime.GetIntDate(Expiry.Value);
                claims.Add(new Claim("exp", expDt.ToString(), ClaimValueTypes.Integer));
            }

            if (IssuedAt.HasValue)
            {
                var iatDt = EpochTime.GetIntDate(IssuedAt.Value);
                claims.Add(new Claim("iat", iatDt.ToString(), ClaimValueTypes.Integer));
            }

            if (Issuer != null) claims.Add(new Claim("iss", Issuer));

            if (Audiences != null)
            {
                foreach (var audience in Audiences)
                {
                    claims.Add(new Claim("aud", audience));
                }
            }

            return claims.ToArray();
        }

        public BaseClaimSet WithExpiry(DateTime expiry)
        {
            return new BaseClaimSet
            {
                Expiry = expiry,
                IssuedAt = IssuedAt,
                Audiences = Audiences.ToArray(),
                Issuer = Issuer,
                Subject = Subject
            };
        }
    }
}
