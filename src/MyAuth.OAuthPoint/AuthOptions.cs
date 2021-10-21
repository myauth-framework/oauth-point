namespace MyAuth.OAuthPoint
{
    public class AuthTimingsOptions
    {
        public int LoginExpirySeconds { get; set; } = 60;
        public int SessionExpiryDays { get; set; } = 10;
        public string ClientPasswordSalt { get; set; }
        
    }

    public class AuthStoringOptions
    {
        public string ClientPasswordSalt { get; set; }

    }

    public class AuthEndpointsOptions
    {
        public string LoginEndpoint { get; set; }
        public string DefaultErrorEndpoint { get; set; }

    }

    public class TokenIssuingOptions
    {
        public string Issuer { get; set; }
        public int AccessTokenExpirySeconds { get; set; }
        public string SignSymmetricKey { get; set; }
        public string SignCertificateSubjectCn { get; set; }
    }
}
