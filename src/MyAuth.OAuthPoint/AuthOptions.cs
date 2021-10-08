namespace MyAuth.OAuthPoint
{
    public class AuthOptions
    {
        public string LoginEndpoint { get; set; }

        public int LoginInitiationExpirySeconds { get; set; } = 60;
        public int LoginSessionExpiryDays { get; set; } = 10;

        public string DefaultErrorEndpoint { get; set; }
    }
}
