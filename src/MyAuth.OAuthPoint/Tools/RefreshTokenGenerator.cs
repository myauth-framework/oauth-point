using System;
using System.Text;

namespace MyAuth.OAuthPoint.Tools
{
    static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            var guid = Guid.NewGuid().ToString("N");
            var binGuid = Encoding.UTF8.GetBytes(guid);
            var base64Guid = Convert.ToBase64String(binGuid);

            return base64Guid;
        }
    }
}