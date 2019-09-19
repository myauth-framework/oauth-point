using System;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace MyAuth.OAuthPoint.Tools
{
    class RefreshToken
    {
        public DateTime NotAfter { get; set; }
        
        public string Body { get; set; }
        
        public static RefreshToken Generate(int lifetimeDays)
        {
            var guid = Guid.NewGuid().ToString("N");
            var binGuid = Encoding.UTF8.GetBytes(guid);
            var base64Guid = WebEncoders.Base64UrlEncode(binGuid);

            var notAfter = DateTime.Now.AddDays(lifetimeDays);
            
            return new RefreshToken
            {
                NotAfter = notAfter,
                Body = base64Guid
            };
        }
    }
}