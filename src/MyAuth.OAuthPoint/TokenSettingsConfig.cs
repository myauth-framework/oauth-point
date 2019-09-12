namespace MyAuth.OAuthPoint
{
    public class TokenIssuingOptions
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int AccessTokenLifeTimeMin { get; set; }
        public int RefreshTokenLifeTimeDays { get; set; }
    }
}