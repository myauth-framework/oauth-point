using System;

namespace MyAuth.OAuthPoint.Tools
{
    static class ScopeStringParser
    {
        public static string[] Parse(string scopesString)
        {
            return scopesString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}