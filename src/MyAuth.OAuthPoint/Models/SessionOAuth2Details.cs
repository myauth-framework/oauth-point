namespace MyAuth.OAuthPoint.Models
{
    public class SessionOAuth2Details
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public string AuthorizationCode { get; set; }

        public AuthorizationRequestProcessingError ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}