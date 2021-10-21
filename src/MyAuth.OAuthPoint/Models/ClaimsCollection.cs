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
    public class ClaimsCollection : Dictionary<string, ClaimValue>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ClaimsCollection"/>
        /// </summary>
        public ClaimsCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimsCollection"/>
        /// </summary>
        public ClaimsCollection(IDictionary<string, ClaimValue> initial): base(initial)
        {
        }
    }
}