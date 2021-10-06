using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    public class AddressClaim
    {
        /// <summary>
        /// Full mailing address, formatted for display or use on a mailing label. This field MAY contain multiple lines, separated by newlines. Newlines can be represented either as a carriage return/line feed pair ("\r\n") or as a single line feed character ("\n").
        /// </summary>
        [JsonProperty("formatted")]
        public string Formatted { get; set; }
        /// <summary>
        /// Full street address component, which MAY include house number, street name, Post Office Box, and multi-line extended street address information. This field MAY contain multiple lines, separated by newlines. Newlines can be represented either as a carriage return/line feed pair ("\r\n") or as a single line feed character ("\n").
        /// </summary>
        [JsonProperty("street_address")]
        public string StreetAddress{ get; set; }
        /// <summary>
        /// City or locality component.
        /// </summary>
        [JsonProperty("locality")]
        public string Locality{ get; set; }
        /// <summary>
        /// State, province, prefecture, or region component.
        /// </summary>
        [JsonProperty("region")]
        public string Region{ get; set; }
        /// <summary>
        /// Zip code or postal code component.
        /// </summary>
        [JsonProperty("postal_code")]
        public string PostalCode{ get; set; }
        /// <summary>
        /// Country name component.
        /// </summary>
        [JsonProperty("country")]
        public string Country{ get; set; }
    }
}