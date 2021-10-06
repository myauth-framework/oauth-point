namespace MyAuth.OAuthPoint.Models
{
    public class LoginInitDetails
    {
        public LoginSessionError Error{ get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public string AuthorizationCode { get; set; }
    }
}