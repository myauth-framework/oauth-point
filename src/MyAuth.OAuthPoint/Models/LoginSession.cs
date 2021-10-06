using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginSession
    {
        public string Id { get; set; }

        public LoginInitDetails InitDetails { get; set; }

        public AuthorizedUserInfo AuthorizedUserInfo { get; set; }
    }
}