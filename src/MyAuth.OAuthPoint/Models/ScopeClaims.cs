using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models 
#endif
{
    /// <summary>
    /// The base for object with claim properties
    /// </summary>
    public abstract class ScopeClaims
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ScopeClaims"/>
        /// </summary>
        protected ScopeClaims()
        {
            
        }

        /// <summary>
        /// Converts claims to <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        public IDictionary<string, JObject> ToDictionary()
        {
            return GetType()
                .GetProperties()
                .Select(p => new {Property = p, Attribute = p.GetCustomAttribute<JsonPropertyAttribute>()})
                .Where(p => p.Attribute != null)
                .ToDictionary(p => p.Attribute.PropertyName, p => JObject.FromObject(p.Property.GetValue(this)));
        }
    }
}