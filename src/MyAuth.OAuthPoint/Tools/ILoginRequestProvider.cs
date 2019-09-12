using System.Collections.Generic;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tools
{
    interface ILoginRequestProvider
    {
        Task<LoginRequest> Provide();
    }

    class AuthCodeBasedLoginRequestProvider : ILoginRequestProvider
    {
        private readonly string _authCode;
        private readonly ILoginRegistry _loginRegistry;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthCodeBasedLoginRequrestProvider"/>
        /// </summary>
        public AuthCodeBasedLoginRequestProvider(string authCode, ILoginRegistry loginRegistry)
        {
            _authCode = authCode;
            _loginRegistry = loginRegistry;
        }
        
        public async Task<LoginRequest> Provide()
        {
            return await _loginRegistry.Get(_authCode);
        }
    }
    
    class RefreshTokenBasedLoginRequestProvider : ILoginRequestProvider
    {
        private readonly string _refreshToken;
        private readonly IRefreshTokenRegistry _refreshTokenRegistry;

        /// <summary>
        /// Initializes a new instance of <see cref="RefreshTokenBasedLoginRequestProvider"/>
        /// </summary>
        public RefreshTokenBasedLoginRequestProvider(string refreshToken, IRefreshTokenRegistry refreshTokenRegistry)
        {
            _refreshToken = refreshToken;
            _refreshTokenRegistry = refreshTokenRegistry;
        }
        
        public async Task<LoginRequest> Provide()
        {
            return await _refreshTokenRegistry.Get(_refreshToken);
        }
    }
}