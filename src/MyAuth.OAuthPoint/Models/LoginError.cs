namespace MyAuth.OAuthPoint.Models
{
    public class LoginError
    {
        public LoginErrorType Type { get; set; }
        public string Description { get; set; }
    }
}