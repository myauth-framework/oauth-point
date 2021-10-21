using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyAuth.OAuthPoint.Tools
{
    static class BasicAuthParser
    {
        public static bool TryParse(string value, out string username, out string password)
        {
            username = null;
            password = null;
            
            string encodedUsernamePassword = value;
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int separatorIndex = usernamePassword.IndexOf(':');

            if (separatorIndex < 0)
                return false;

            username = usernamePassword.Substring(0, separatorIndex);
            password = usernamePassword.Substring(separatorIndex + 1);

            return true;
        }
    }
}
