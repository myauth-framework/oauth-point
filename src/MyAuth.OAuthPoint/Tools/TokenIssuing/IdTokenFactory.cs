using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Tools.TokenIssuing
{
    class IdTokenFactory
    {
        private readonly BaseClaimSet _baseClaimSet;
        private readonly ClaimsCollection _userInfo;

        public IdTokenFactory(BaseClaimSet baseClaimSet, ClaimsCollection userInfo)
        {
            _baseClaimSet = baseClaimSet;
            _userInfo = userInfo;
        }

        public string Create(X509Certificate2 issuerCertificate)
        {
            if (issuerCertificate == null) 
                throw new ArgumentNullException(nameof(issuerCertificate));

            var payloadModel = new JwtPayloadModel
            {
                _baseClaimSet.ToClaimsCollection()
            };

            if (_userInfo != null)
                payloadModel.Add(_userInfo);

            var payloadModelObject = payloadModel.ToModelObject();
            var stringPayload = JsonConvert.SerializeObject(payloadModelObject);
            var payload = JwtPayload.Deserialize(stringPayload);

            var signingCredentials = new X509SigningCredentials(issuerCertificate);

            var header = new JwtHeader(signingCredentials);

            JwtSecurityToken t = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(t);
        }
    }
}
