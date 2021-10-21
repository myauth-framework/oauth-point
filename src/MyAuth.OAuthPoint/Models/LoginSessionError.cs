using MyAuth.OAuthPoint.Tools;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginSessionError
    {
        [JsonConverter(typeof(EnumNameJsonConverter))]
        public AuthorizationRequestProcessingError Error { get; set; }
        public string Description { get; set; }
    }
}