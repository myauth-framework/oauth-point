using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;

namespace MyAuth.OAuthPoint.Services
{
    interface IX509CertificateProvider
    {
        X509Certificate2 ProvideIssuerCertificate();
    }

    class X509CertificateProvider : IX509CertificateProvider
    {
        private readonly TokenIssuingOptions _tokenIssuingOptions;

        public X509CertificateProvider(IOptions<TokenIssuingOptions> tokenIssuingOptions)
            :this(tokenIssuingOptions.Value)
        {
            
        }

        public X509CertificateProvider(TokenIssuingOptions tokenIssuingOptions)
        {
            _tokenIssuingOptions = tokenIssuingOptions;
        }

        public X509Certificate2 ProvideIssuerCertificate()
        {
            throw new System.NotImplementedException();
        }
    }
}
