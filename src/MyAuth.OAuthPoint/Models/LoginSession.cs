using System;
using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginSession
    {
        public string Id { get; set; }

        public string ClientId { get; set; }
        public DateTime Expiry { get; set; }

        public LoginInitDetails InitDetails { get; set; }

        public AuthorizedUserInfo AuthorizedUserInfo { get; set; }
    }
}