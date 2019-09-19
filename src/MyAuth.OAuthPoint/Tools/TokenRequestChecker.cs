using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;

namespace MyAuth.OAuthPoint.Tools
{
    class TokenRequestChecker
    {
        public const string AuthCodeGrantType = "authorization_code";
        public const string RefreshTokenGrantType = "refresh_token";
        
        private static readonly string[] SupportedGrantTypes = {AuthCodeGrantType, RefreshTokenGrantType};
        
        private readonly TokenRequest _tokenRequest;

        public TokenRequestChecker(TokenRequest tokenRequest)
        {
            _tokenRequest = tokenRequest;
        }
        
        public ErrorTokenResponse CheckState()
        {
            if (_tokenRequest == null)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Request id empty"
                };
            if (_tokenRequest.ClientId == null)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "ClientId is required"
                };
            if (_tokenRequest.GrantType == AuthCodeGrantType && _tokenRequest.AuthCode == null)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Authorization code is required"
                };
            
            if (_tokenRequest.GrantType == RefreshTokenGrantType && _tokenRequest.RefreshToken == null)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Refresh token is required"
                };
            
            if (!SupportedGrantTypes.Contains(_tokenRequest.GrantType))
                return new ErrorTokenResponse{ErrorCode = TokenResponseErrorCode.UnsupportedGrantType};

            return null;
        }
        
        public ErrorTokenResponse CheckLoginRequest(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidClient, 
                    ErrorDescription = "Login event not found"
                };

            if (loginRequest.ClientId != _tokenRequest.ClientId)
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Wrong ClientId"
                };
            
            if (!string.IsNullOrEmpty(loginRequest.CodeChallenge) && string.IsNullOrEmpty(_tokenRequest.CodeVerifier))
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Wrong code verifier"
                };

            if (!string.IsNullOrEmpty(_tokenRequest.CodeVerifier) &&
                !CheckCodeVerifier(loginRequest.CodeChallenge, _tokenRequest.CodeVerifier))
                return new ErrorTokenResponse
                {
                    ErrorCode = TokenResponseErrorCode.InvalidRequest,
                    ErrorDescription = "Wrong proof key"
                };

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