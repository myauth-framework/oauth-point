using Newtonsoft.Json;

#if MYAUTH_CLIENT

using MyAuth.OAuthPoint.Client.Tools;

namespace MyAuth.OAuthPoint.Client.Models
#else

using MyAuth.OAuthPoint.Tools;

namespace MyAuth.OAuthPoint.Models.DataContract 
#endif
{
    public class LoginSuccessRequest
    {
        public string Subject { get; set; }
        public ScopeClaims[] IdentityScopes { get; set; }
        public ClaimsCollection AccessClaims { get; set; }
    }
}