namespace MyAuth.OAuthPoint
{
    public class AuthOptions
    {
        public string LoginEndpoint { get; set; }

        public int LoginExpiryDays { get; set; } = 10;

        public string DefaultErrorEndpoint { get; set; }
    }
}
