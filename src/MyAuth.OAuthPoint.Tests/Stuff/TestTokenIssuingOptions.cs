namespace MyAuth.OAuthPoint.Tests
{
    static class TestTokenIssuingOptions
    {
        public static readonly TokenIssuingOptions Options = new TokenIssuingOptions
        {
            Issuer= "MyAuth.OAuthPoint",
            AccessTokenLifeTimeMin = 1,
            RefreshTokenLifeTimeDays = 10,
            Secret = "qwertyqwertyqwerty"
        };
    }
}