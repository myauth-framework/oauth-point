namespace MyAuth.OAuthPoint
{
    public class AuthOptions
    {
        public string LoginEndpoint { get; set; }

        public int LoginExpirySeconds { get; set; } = 60;
        public int SessionExpiryDays { get; set; } = 10;

        public string DefaultErrorEndpoint { get; set; }

        public string ClientPasswordSalt { get; set; }
    }
}
