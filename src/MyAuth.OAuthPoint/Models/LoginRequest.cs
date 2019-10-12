
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
        public string[] Audience { get; set; }
        public Claim[] Claims { get; set; }


        public class Claim
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}