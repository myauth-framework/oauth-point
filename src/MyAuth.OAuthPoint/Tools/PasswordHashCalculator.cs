using System;
using System.Security.Cryptography;
using System.Text;

namespace MyAuth.OAuthPoint.Tools
{
    class PasswordHashCalculator
    {
        private readonly byte[] _salt;

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
