using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyAuth.OAuthPoint.Models
{
    public class TokenRequest
    {
        [BindProperty(Name="grant_type")]
        public string GrantType { get; set; }
        [BindProperty(Name="code_verifier")]
        public string CodeVerifier { get; set; }
        
        [BindProperty(Name="code")]
        public string AuthCode { get; set; }
        
        [BindProperty(Name="client_id")]
        public string ClientId { get; set; }

        public FormUrlEncodedContent ToUrlEncodedContent()
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", GrantType},
                { "code", AuthCode},
                { "client_id", ClientId},
                { "code_verifier", CodeVerifier}
            };

            return new FormUrlEncodedContent(dict);
        }
    }
}