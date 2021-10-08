using System.Collections.Immutable;

namespace MyAuth.OAuthPoint.Models
{
    public static class StandardClaimsScopes
    {
        public const string Phone = "phone";
        public const string Address = "address";
        public const string Profile  = "profile";
        public const string Email  = "email";

        public static readonly ImmutableArray<string> Scopes = new ImmutableArray<string>()
        {
            Address, Email, Phone, Profile
        };
    }
}
