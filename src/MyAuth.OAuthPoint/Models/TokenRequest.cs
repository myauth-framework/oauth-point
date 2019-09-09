using Microsoft.AspNetCore.Mvc;

namespace MyAuth.OAuthPoint.Models
{
    public class TokenRequest
    {
        [BindProperty(Name="code_verifier")]
        public string CodeVerifier { get; set; }
        
        [BindProperty(Name="code")]
        public string AuthCode { get; set; }
        
        [BindProperty(Name="client_id")]
        public string ClientId { get; set; }
    }
}