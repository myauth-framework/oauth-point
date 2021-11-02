using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using MyLab.Log;

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
            if(!File.Exists(_tokenIssuingOptions.SignCertificateCertPath))
                throw new FileNotFoundException("Certificate file not found")
                    .AndFactIs("path", _tokenIssuingOptions.SignCertificateCertPath);
            if (!File.Exists(_tokenIssuingOptions.SignCertificateKeyPath))
                throw new FileNotFoundException("Certificate key file not found")
                    .AndFactIs("path", _tokenIssuingOptions.SignCertificateKeyPath);

            return X509Certificate2.CreateFromEncryptedPemFile(
                _tokenIssuingOptions.SignCertificateCertPath,
                _tokenIssuingOptions.SignCertificatePassword,
                _tokenIssuingOptions.SignCertificateKeyPath);
        }
    }
}
