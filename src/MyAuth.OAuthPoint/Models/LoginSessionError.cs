namespace MyAuth.OAuthPoint.Models
{
    public class LoginSessionError
    {
        public AuthorizationRequestProcessingError Error { get; set; }
        public string Description { get; set; }
    }
}