using System;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    public class IdentityToken
    {
        static DateTime EpochTime = new DateTime(1970, 01,01);
        
        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; set; }
        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "exp")]
        public double ExpirationTime { get; set; }
        [JsonProperty(PropertyName = "myauth:roles")]
        public string[] Roles { get; set; }
        [JsonProperty(PropertyName = "myauth:climes")]
        public Clime[] Climes { get; set; }

        public void SetExpirationTime(DateTime dateTime)
        {
            ExpirationTime = (dateTime - EpochTime).TotalSeconds;
            //ExpirationTime = Math.Round((dateTime - EpochTime).TotalSeconds, MidpointRounding.AwayFromZero);
        }

        public DateTime GetExpirationTime()
        {
            return EpochTime.AddSeconds(ExpirationTime);
        }
    }
}