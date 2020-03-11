namespace MyAuth.OAuthPoint.Tests
{
    static class TestTokenIssuingOptions
    {
        public static readonly TokenIssuingOptions Options = new TokenIssuingOptions
        {
            Issuer= "MyAuth.OAuthPoint",
            AccessTokenLifeTimeMin = 100000,
            RefreshTokenLifeTimeDays = 100000,
            Secret = "qwertyqwertyqwerty"
        };
    }
}