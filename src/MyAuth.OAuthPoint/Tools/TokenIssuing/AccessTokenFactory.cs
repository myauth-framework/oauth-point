using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;

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
            
            var payloadModel = new PayloadModel
            {
                _baseClaimSet.ToArray()
            };
            
            if(_addClaims != null)
                payloadModel.Add(_addClaims);

            if (Scope != null)
                payloadModel.Add(new Claim("scope", Scope));

            var payloadModelObject = payloadModel.ToModelObject();
            var stringPayload = JsonConvert.SerializeObject(payloadModelObject);

            var payload = JwtPayload.Deserialize(stringPayload);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var header = new JwtHeader(new SigningCredentials(secKey, "HS256"));

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            
            return tokenHandler.WriteToken(t);
        }

        class PayloadModel : Dictionary<string, List<ClaimValue>>
        {
            public void Add(IEnumerable<Claim> claims)
            {
                foreach (var claim in claims)
                {
                    Add(claim);
                }
            }
            
            public void Add(ClaimsCollection claims)
            {
                foreach (var claim in claims)
                {
                    var list = RetrieveList(claim.Key);
                    list.Add(claim.Value);
                }
            }

            public void Add(Claim claim)
            {
                var list = RetrieveList(claim.Type);
                list.Add(new ClaimValue(claim.Value));
            }

            List<ClaimValue> RetrieveList(string key)
            {
                if (!TryGetValue(key, out var list))
                {
                    list = new List<ClaimValue>();
                    Add(key, list);
                }

                return list;
            }

            public Dictionary<string, object> ToModelObject()
            {
                var newObj = new Dictionary<string, object>();

                foreach (var item in this)
                {
                    newObj.Add(item.Key, item.Value.Count > 1 
                        ? (object)item.Value.ToArray()
                        : (object)item.Value.First());
                }

                return newObj;
            }
        }
    }
}
