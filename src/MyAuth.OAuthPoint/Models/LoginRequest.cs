using MyAuth.Common;

namespace MyAuth.OAuthPoint.Models
{
    public class LoginRequest
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string Subject { get; set; }
        public string CodeChallenge { get; set; }
        public string CodeChallengeMethod { get; set; }
        public string[] Roles { get; set; } 
        public Clime[] Climes { get; set; }
    }
}