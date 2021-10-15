using System.Collections.Generic;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// Information about authorized user
    /// </summary>
    public class AuthorizedSubjectInfo
    {
        /// <summary>
        /// Subject (user) identifier
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Claims for `profile` scope
        /// </summary>
        public ProfileScopeClaims Profile { get; set; }
        /// <summary>
        /// Claims for `address` scope
        /// </summary>
        public AddressScopeClaims Address { get; set; }
        /// <summary>
        /// Claims for `email` scope
        /// </summary>
        public EmailScopeClaims Email { get; set; }
        /// <summary>
        /// Claims for `phone` scope
        /// </summary>
        public PhoneScopeClaims Phone{ get; set; }
        /// <summary>
        /// Claims for custom scopes
        /// </summary>
        public Dictionary<string, CustomScopeClaims> CustomScopes { get; set; }
    }
}
