using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MyAuth.OAuthPoint.Services
{
    class PasswordHashCalculator
    {
        private readonly byte[] _salt;

        public PasswordHashCalculator(IOptions<AuthStoringOptions> authStoringOptions)
            :this(authStoringOptions.Value.ClientPasswordSalt)
        {
            
        }

        public PasswordHashCalculator(string salt)
        {
            _salt = Encoding.UTF8.GetBytes(salt);
        }

        public string CalcHexPasswordMd5(string password)
        {
            var binPass = Encoding.UTF8.GetBytes(password);

            var hmacMd5 = new HMACMD5(_salt);
            var hash = hmacMd5.ComputeHash(binPass);

            return Convert.ToHexString(hash).ToLower();
        }
    }
}
