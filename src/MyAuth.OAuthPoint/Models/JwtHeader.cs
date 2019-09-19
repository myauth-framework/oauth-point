using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyAuth.OAuthPoint.Models
{
    public class JwtHeader
    {
        [JsonProperty(PropertyName = "alg")]
        public string Algorithm { get; set; }
        [JsonProperty(PropertyName = "typ")]
        public string Type { get; set; }
    }
}