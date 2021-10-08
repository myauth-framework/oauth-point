using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAuth.OAuthPoint.Tools
{
    public static class JsonClaimValueParser
    {
        public static JObject Parse(string str)
        {
            if (str == null) return (JObject)null;
            if (str.StartsWith("{") || str.StartsWith("["))
                return new JObject(JsonConvert.DeserializeObject(str));
            return new JObject(str);
        }
    }
}
