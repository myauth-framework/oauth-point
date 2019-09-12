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
        [BindProperty(Name="refresh_token")]
        public string RefreshToken { get; set; }
        
        [BindProperty(Name="client_id")]
        public string ClientId { get; set; }

        public FormUrlEncodedContent ToUrlEncodedContent()
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", GrantType},
                { "client_id", ClientId},
                { "code_verifier", CodeVerifier}
            };
            
            if(AuthCode != null)
                dict.Add("code", AuthCode);
            if(RefreshToken != null)
                dict.Add("refresh_token", RefreshToken);

            return new FormUrlEncodedContent(dict);
        }
    }
}