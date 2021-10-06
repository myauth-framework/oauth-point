using System.Collections.Generic;

namespace MyAuth.OAuthPoint.Models
{
    public class AuthorizedUserInfo
    {
        public string Subject { get; set; }

        public ProfileScopeClaims Profile { get; set; }
        public AddressScopeClaims Address { get; set; }
        public EmailScopeClaims Email { get; set; }
        public PhoneScopeClaims Phone{ get; set; }

        public Dictionary<string, CustomScopeClaims> CustomScopes { get; set; }
    }
}
