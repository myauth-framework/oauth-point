using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// Contains claim for custom scope
    /// </summary>
    public class CustomScopeClaims : Dictionary<string, JObject>
    {

    }
}