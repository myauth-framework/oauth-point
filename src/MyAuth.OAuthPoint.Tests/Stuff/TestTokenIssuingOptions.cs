namespace MyAuth.OAuthPoint.Tests
{
    static class TestTokenIssuingOptions
    {
        public static readonly TokenIssuingOptions Options = new TokenIssuingOptions
        {
            Issuer= "MyAuth.OAuthPoint",
            AccessTokenLifeTimeMin = 1000,
            RefreshTokenLifeTimeDays = 1000,
            Secret = "qwertyqwertyqwerty"
        };
    }
}