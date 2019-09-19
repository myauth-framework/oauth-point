namespace MyAuth.OAuthPoint.Tests
{
    static class TestTokenIssuingOptions
    {
        public static readonly TokenIssuingOptions Options = new TokenIssuingOptions
        {
            Issuer= "MyAuth.OAuthPoint",
            AccessTokenLifeTimeMin = 100000000,
            RefreshTokenLifeTimeDays = 100000,
            Secret = "qwerty"
        };
    }
}