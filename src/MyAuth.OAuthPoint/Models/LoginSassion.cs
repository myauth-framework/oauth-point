using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginSassion
    {
        public LoginStateError Error{ get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

        public string AuthorizationCode { get; set; }
    }

    public class LoginStateError
    {
        public AuthorizationRequestProcessingError Error { get; set; }
        public string Description { get; set; }
    }
}