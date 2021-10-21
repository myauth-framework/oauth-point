using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models
#endif
{
    /// <summary>
    /// Contains claims with json values
    /// </summary>
    public class JsonClaimsCollection : Dictionary<string, JObject>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonClaimsCollection"/>
        /// </summary>
        public JsonClaimsCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonClaimsCollection"/>
        /// </summary>
        public JsonClaimsCollection(IDictionary<string, JObject> initial): base(initial)
        {
        }
    }
}