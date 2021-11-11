using System;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class BaseClaimSet
    {
        public string Subject { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime? IssuedAt { get; set; }
        public string Issuer { get; set; }
        public string[] Audiences { get; set; }

        public ClaimsCollection ToClaimsCollection()
        {
            var claims = new ClaimsCollection();

            if(Subject != null) claims.Add("sub", new ClaimValue(Subject));
            
            if (Expiry.HasValue)
            {
                var expDt = EpochTime.GetIntDate(Expiry.Value);
                claims.Add("exp", new ClaimValue(expDt));
            }

            if (IssuedAt.HasValue)
            {
                var iatDt = EpochTime.GetIntDate(IssuedAt.Value);
                claims.Add("iat", new ClaimValue(iatDt));
            }

            if (Issuer != null) claims.Add("iss", new ClaimValue(Issuer));

            if (Audiences != null && Audiences.Length != 0)
            {
                claims.Add("aud",
                    Audiences.Length == 1
                        ? new ClaimValue(Audiences[0])
                        : new ClaimValue(JArray.FromObject(Audiences)));
            }

            return claims;
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
