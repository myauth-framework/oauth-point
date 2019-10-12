using System.Net;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using Xunit;

namespace MyAuth.OAuthPoint.Tests
{
    public partial class TokenControllerBehavior
    {
        [Theory]
        [InlineData("Wrong authCode", "foo", "bar", "baz", TokenResponseErrorCode.InvalidClient)]
        [InlineData("Wrong clientId", TestLoginRegistry.TestAuthCode, "bar", "baz",
            TokenResponseErrorCode.InvalidRequest)]
        [InlineData("Wrong code verifier", TestLoginRegistry.TestAuthCode, TestLoginRegistry.TestClientId, "baz",
            TokenResponseErrorCode.InvalidRequest)]
        [InlineData("Empty code verifier", TestLoginRegistry.TestAuthCode, TestLoginRegistry.TestClientId, "",
            TokenResponseErrorCode.InvalidRequest)]
        public async Task ShouldFailInvalidRequests(
            string desc,
            string authCode,
            string clientId,
            string codeVerifier,
            TokenResponseErrorCode expectedCode)
        {
            //Arrange
            var request = new TokenRequest
            {
                AuthCode = authCode,
                ClientId = clientId,
                CodeVerifier = codeVerifier,
                GrantType = "authorization_code"
            };

            //Act
            var respError = await IssueToken<ErrorTokenResponse>(request: request);

            if (respError.Msg.ErrorDescription != null)
                _output.WriteLine(respError.Msg.ErrorDescription);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, respError.Code);
            Assert.Equal(expectedCode, respError.Msg.ErrorCode);
        }

        [Fact]
        public async Task ShouldPassValidRequests()
        {
            //Arrange

            //Act
            var resp = await IssueToken<SuccessTokenResponse>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.Code);
        }

        [Fact]
        public async Task ShouldIssueValidToken()
        {
            //Act
            var resp = await IssueToken<SuccessTokenResponse>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Equal("bearer", resp.Msg.TokenType);
            Assert.Equal(60, resp.Msg.ExpiresIn);
            CheckRefreshToken(resp.Msg.RefreshToken);
            CheckAccessToken(resp.Msg.AccessToken);
        }
    }
}