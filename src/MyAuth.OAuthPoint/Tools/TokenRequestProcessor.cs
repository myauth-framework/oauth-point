using System;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Services;

namespace MyAuth.OAuthPoint.Tools
{
    class TokenRequestProcessor
    {
        private readonly Lazy<TokenRequestChecker> _tokenRequestChecker;

        public TokenRequest TokenRequest { get; }

        public LoginRequest LoginRequest { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TokenRequestProcessor"/>
        /// </summary>
        public TokenRequestProcessor(TokenRequest tokenRequest)
        {
            TokenRequest = tokenRequest;

            _tokenRequestChecker = new Lazy<TokenRequestChecker>(() => new TokenRequestChecker(tokenRequest));
        }

        public ErrorTokenResponse CheckState()
        {
            return _tokenRequestChecker.Value.CheckState();
        }

        public async Task<ErrorTokenResponse> LoadAndCheckLoginRequest(ILoginRegistry loginRegistry, IRefreshTokenRegistry refreshTokenRegistry)
        {
            ILoginRequestProvider loginReqProvider;

            switch (TokenRequest.GrantType)
            {
                case TokenRequestChecker.AuthCodeGrantType:
                    loginReqProvider = new AuthCodeBasedLoginRequestProvider(TokenRequest.AuthCode, loginRegistry);
                    break;
                case TokenRequestChecker.RefreshTokenGrantType:
                    loginReqProvider = new RefreshTokenBasedLoginRequestProvider(TokenRequest.RefreshToken, refreshTokenRegistry);
                    break;
                default:
                    throw new IndexOutOfRangeException("Unsupported grant type");
            }


            LoginRequest = await loginReqProvider.Provide();

            return _tokenRequestChecker.Value.CheckLoginRequest(LoginRequest);
        }

        public async Task RegisterRefreshToken(RefreshToken refreshToken, IRefreshTokenRegistry refreshTokenRegistry)
        {
            await refreshTokenRegistry.Register(refreshToken.Body, new RefreshTokenDescription
            {
                LoginRequest = LoginRequest,
                NotAfter = refreshToken.NotAfter
            });
        }
        public SuccessTokenResponse CreateSuccessResponse(
            RefreshToken refreshToken, 
            string secret,
            string issuer, 
            int accessTokenLifeTimeMin)
        {
            var accessTokenBuilder=  new AccessTokenBuilder();

            accessTokenBuilder.CreateHeader(secret);
            accessTokenBuilder.CreatePayload(LoginRequest, issuer, LoginRequest.Audience, accessTokenLifeTimeMin);

            return new SuccessTokenResponse
            {
                AccessToken = accessTokenBuilder.BuildString(),
                ExpiresIn = accessTokenLifeTimeMin * 60,
                RefreshToken = refreshToken.Body
            };
        }
    }
}
