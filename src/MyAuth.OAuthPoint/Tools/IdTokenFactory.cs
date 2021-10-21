using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    public class IdTokenFactory
    {
        public string[] RequiredScopes { get; set; }

        public ScopeClaims[] Scopes{ get; set; }
    }
}
