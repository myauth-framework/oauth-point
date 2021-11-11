using System;
using System.IdentityModel.Tokens.Jwt;
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
            
            var payloadModel = new JwtPayloadModel
            {
                _baseClaimSet.ToClaimsCollection()
            };
            
            if(_addClaims != null)
                payloadModel.Add(_addClaims);

            if (Scope != null)
                payloadModel.Add("scope", new ClaimValue(Scope));

            var payloadModelObject = payloadModel.ToModelObject();
            var stringPayload = JsonConvert.SerializeObject(payloadModelObject);
            var payload = JwtPayload.Deserialize(stringPayload);

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var header = new JwtHeader(new SigningCredentials(secKey, "HS256"));

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            
            return tokenHandler.WriteToken(t);
        }
    }
}
