using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Tools
{
    public class AccessTokenBuilder
    {
        private static readonly string TokenHeaderBase64;

        private readonly HashAlgorithm _hashAlg;
        public string Secret { get; }
        public string Issuer { get; set; }
        public int LifeTimeMin { get; set; }

        static AccessTokenBuilder()
        {
            var header = new JwtHeader
            {
                Algorithm = "HS256",
                Type = "JWT"
            };
            var headerStr = JsonConvert.SerializeObject(header);
            var binHeader = Encoding.UTF8.GetBytes(headerStr);
            TokenHeaderBase64 = WebEncoders.Base64UrlEncode(binHeader);
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="AccessTokenBuilder"/>
        /// </summary>
        public AccessTokenBuilder(string secret)
        {
            Secret = secret;

            var binSecret = Encoding.UTF8.GetBytes(secret);
            _hashAlg = new HMACSHA256(binSecret);
        }
        
        public string Build(LoginRequest loginRequest)
        {
            var idToken = new IdentityToken
            {
                Issuer = Issuer,
                Subject = loginRequest.ClientId,
                Roles = loginRequest.Roles,
                Climes = loginRequest.Climes
            };
            
            idToken.SetExpirationTime(DateTime.Now.AddMinutes(LifeTimeMin));

            var strIdToken = JsonConvert.SerializeObject(idToken);
            var binIdToken = Encoding.UTF8.GetBytes(strIdToken);
            var base64IdToken = WebEncoders.Base64UrlEncode(binIdToken);

            var dataForHashing = TokenHeaderBase64 + "." + base64IdToken;
            var binDataForHashing = Encoding.UTF8.GetBytes(dataForHashing);
            
            var idTokenHash = _hashAlg.ComputeHash(binDataForHashing);
            var idTokenHashBase64 = WebEncoders.Base64UrlEncode(idTokenHash);

            var accessToken = TokenHeaderBase64 + "." + base64IdToken + "." + idTokenHashBase64;

            return accessToken;
        }
    }
}