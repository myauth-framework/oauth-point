using System;
using DotRedis.Commands.Keys;
using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    public class AccessToken
    {
        static DateTime EpochTime = new DateTime(1970, 01,01);
        
        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "exp")]
        public double ExpirationTime { get; set; }
        [JsonProperty(PropertyName = "myauth:roles")]
        public string[] Roles { get; set; }

        public void SetExpirationTime(DateTime dateTime)
        {
            ExpirationTime = (dateTime - EpochTime).TotalSeconds;
        }
    }
}