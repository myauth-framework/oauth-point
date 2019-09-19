using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    public class ErrorTokenResponse
    {
        [JsonProperty(PropertyName = "error")]
        public TokenResponseErrorCode ErrorCode { get; set; }
        
        [JsonProperty(PropertyName = "error_description")]
        public string ErrorDescription { get; set; }
    }
}