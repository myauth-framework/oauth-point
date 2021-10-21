#if MYAUTH_CLIENT
using MyAuth.OAuthPoint.Client.Tools;

namespace MyAuth.OAuthPoint.Client.Models
#else
using MyAuth.OAuthPoint.Tools;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models.DataContract
#endif
{
    /// <summary>
    /// Error response fro token request 
    /// </summary>
    /// <remarks>https://datatracker.ietf.org/doc/html/rfc6749#section-5.2</remarks>
    public class ErrorTokenResponse
    {
        /// <summary>
        /// REQUIRED.  A single ASCII [USASCII] error code
        /// </summary>
        [JsonProperty("error")]
        [JsonConverter(typeof(EnumNameJsonConverter))]
        public TokenRequestProcessingError AuthError { get; set; }

        /// <summary>
        /// OPTIONAL.  Human-readable ASCII [USASCII] text providing additional information, used to assist the client developer in understanding the error that occurred.
        /// </summary>
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}