using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    /// <summary>
    /// This scope value requests access to the email and email_verified Claims.
    /// </summary>
    public class EmailScopeClaims : ScopeClaims
    {
        /// <summary>
        /// End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322] addr-spec syntax. The RP MUST NOT rely upon this value being unique, as discussed in Section 5.7.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// True if the End-User's e-mail address has been verified; otherwise false. When this Claim Value is true, this means that the OP took affirmative steps to ensure that this e-mail address was controlled by the End-User at the time the verification was performed. The means by which an e-mail address is verified is context-specific, and dependent upon the trust framework or contractual agreements within which the parties are operating.
        /// </summary>
        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }
    }
}