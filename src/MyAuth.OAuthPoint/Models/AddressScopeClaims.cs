using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// This scope value requests access to the address Claim.
    /// </summary>
    public class AddressScopeClaims : ScopeClaims
    {
        /// <summary>
        /// End-User's preferred postal address. The value of the address member is a JSON [RFC4627] structure containing some or all of the members defined in Section 5.1.1.
        /// </summary>
        [JsonProperty("address")]
        public AddressClaim Address { get; set; }
    }
}