using MyLab.Config;

namespace MyAuth.OAuthPoint
{
    [ConfigNode("TokenSettings")]
    public class TokenSettingsConfig
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int AccessTokenLifeTimeMin { get; set; }
        public int RefreshTokenLifeTimeDays { get; set; }
    }
}