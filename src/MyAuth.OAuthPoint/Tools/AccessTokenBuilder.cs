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
        private static string TokenHeaderBase64 = "{\"typ\":\"JWT\",\"alg\":\"HS256\"}";
        
        private static HashAlgorithm hashAlg = SHA256.Create();
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

            var dataForHashing = TokenHeaderBase64 + "." + base64IdToken + Secret;
            var binDataForHashing = Encoding.UTF8.GetBytes(dataForHashing);
            
            var idTokenHash = hashAlg.ComputeHash(binDataForHashing);
            var idTokenHashBase64 = WebEncoders.Base64UrlEncode(idTokenHash);

            var accessToken = TokenHeaderBase64 + "." + base64IdToken + "." + idTokenHashBase64;

            return accessToken;
        }
    }
}