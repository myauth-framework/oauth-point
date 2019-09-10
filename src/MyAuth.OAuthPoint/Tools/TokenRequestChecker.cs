using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    class TokenRequestChecker 
    {
        private readonly ILoginRequestProvider _loginRequestProvider;

        public TokenRequestChecker(ILoginRequestProvider loginRequestProvider)
        {
            _loginRequestProvider = loginRequestProvider;
        }
        
        public async Task<string> Check(TokenRequest tokenRequest)
        {
            if (tokenRequest == null)
                return "Request id empty";
            if (tokenRequest.ClientId == null)
                return "ClientId is required";
            if (tokenRequest.AuthCode == null)
                return "Authorization code is required";
            if (tokenRequest.GrantType != "authorization_code")
                return "Unsupported grant type";

            var loginRequest = await _loginRequestProvider.Provide(tokenRequest.AuthCode);
  
            if (loginRequest == null)
                return "Login event not found";
            
            if (loginRequest.ClientId != tokenRequest.ClientId)
                return "Wrong ClientId";
            
            if (!string.IsNullOrEmpty(loginRequest.CodeChallenge) && string.IsNullOrEmpty(tokenRequest.CodeVerifier))
                return "Wrong code verifier";

            if (!string.IsNullOrEmpty(tokenRequest.CodeVerifier) &&
                !CheckCodeVerifier(loginRequest.CodeChallenge, tokenRequest.CodeVerifier))
                return "Wrong proof key";

            return null;
        }
        
        bool CheckCodeVerifier(string loginRequestCodeChallenge, string tokenRequestCodeVerifier)
        {
            byte[] binCodeVerifier;

            try
            {
                binCodeVerifier = Encoding.UTF8.GetBytes(tokenRequestCodeVerifier);
            }
            catch (Exception)
            {
                return false;
            }
                
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(binCodeVerifier);

            var base64Hash = Convert.ToBase64String(hash);

            return loginRequestCodeChallenge == base64Hash;
        }
    }
}