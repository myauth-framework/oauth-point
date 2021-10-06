using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Models
{
    public abstract class ScopeClaims
    {
        protected ScopeClaims()
        {
            
        }

        public IDictionary<string, JObject> WriteClaims()
        {
            return GetType()
                .GetProperties()
                .Select(p => new {Property = p, Attribute = p.GetCustomAttribute<JsonPropertyAttribute>()})
                .Where(p => p.Attribute != null)
                .ToDictionary(p => p.Attribute.PropertyName, p => JObject.FromObject(p.Property.GetValue(this)));
        }
    }
}