using Newtonsoft.Json;

#if MYAUTH_CLIENT

using MyAuth.OAuthPoint.Client.Tools;

namespace MyAuth.OAuthPoint.Client.Models
#else

using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models.DataContract 
#endif
{
    /// <summary>
    /// Describes login process error
    /// </summary>
    public class LoginErrorRequest
    {
        /// <summary>
        /// OpenID Connect authorization error code
        /// </summary>
        [JsonConverter(typeof(EnumNameJsonConverter))]
        public AuthorizationRequestProcessingError AuthError { get; set; }
        /// <summary>
        /// Human readable description
        /// </summary>
        public string Description { get; set; }
    }
}