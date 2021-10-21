using Newtonsoft.Json;

#if MYAUTH_CLIENT

using MyAuth.OAuthPoint.Client.Tools;

namespace MyAuth.OAuthPoint.Client.Models
#else

using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// Contains scope claims with json value
    /// </summary>
    public class JsonScope
    {
        /// <summary>
        /// Scope identifier
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Claims
        /// </summary>
        public JsonClaimsCollection Claims { get; set; }
    }
}
