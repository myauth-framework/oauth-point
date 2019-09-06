namespace MyAuth.OAuthPoint.Models
{
    public class TokenRequest
    {
        public string CodeVerifier { get; set; }
        public string Code { get; set; }
        public string ClientId { get; set; }
    }
}