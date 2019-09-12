using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using MyAuth.OAuthPoint.Models;
using MyAuth.OAuthPoint.Tools;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    using static TestLoginRegistry;
    public partial class TokenControllerBehavior: IClassFixture<TestWebApplicationFactory>
    {
        [Theory]
        [InlineData("Wrong authCode", "foo", "bar", "baz", TokenResponseErrorCode.InvalidClient)]
        [InlineData("Wrong clientId", TestAuthCode, "bar", "baz", TokenResponseErrorCode.InvalidRequest)]
        [InlineData("Wrong code verifier", TestAuthCode, TestClientId, "baz", TokenResponseErrorCode.InvalidRequest)]
        [InlineData("Empty code verifier", TestAuthCode, TestClientId, "", TokenResponseErrorCode.InvalidRequest)]
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
            var respError = await IssueToken<ErrorTokenResponse>(request:request);
            
            if(respError.Msg.ErrorDescription != null)
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
            Assert.Equal(600, resp.Msg.ExpiresIn);
            CheckRefreshToken(resp.Msg.RefreshToken);
            CheckAccessToken(resp.Msg.AccessToken);
        }

        [Fact]
        public async Task ShouldRefreshToken()
        {
            //Arrange
            var client = _factory.CreateClient();
            var issueTokenResponse = await IssueToken<SuccessTokenResponse>(client);
            
            var request = new TokenRequest
            {
                RefreshToken = issueTokenResponse.Msg.RefreshToken,
                ClientId = TestClientId,
                CodeVerifier = TestCodeVerifier,
                GrantType = "refresh_token"
            };
            var reqContent = request.ToUrlEncodedContent();
            
            //Act
            var refreshResp = await client.PostAsync("/token", reqContent);
            var respStr = await refreshResp.Content.ReadAsStringAsync();
            
            _output.WriteLine(respStr);
            
            //Assert
            Assert.True(refreshResp.IsSuccessStatusCode);

            var resp = JsonConvert.DeserializeObject<SuccessTokenResponse>(respStr);
            
            Assert.Equal("bearer", resp.TokenType);
            Assert.Equal(600, resp.ExpiresIn);
            CheckRefreshToken(resp.RefreshToken);
            CheckAccessToken(resp.AccessToken);
        }

        [Fact]
        public async Task ShouldNotRefreshTokenForInvalidRefreshToken()
        {
            //Arrange
            var client = _factory.CreateClient();

            var invalidRefreshToken = RefreshToken.Generate(1);
            
            var request = new TokenRequest
            {
                RefreshToken = invalidRefreshToken.Body,
                ClientId = TestClientId,
                CodeVerifier = TestCodeVerifier,
                GrantType = "refresh_token"
            };
            var reqContent = request.ToUrlEncodedContent();
            
            //Act
            var refreshResp = await client.PostAsync("/token", reqContent);
            var respStr = await refreshResp.Content.ReadAsStringAsync();
            
            _output.WriteLine(respStr);
            
            //Assert
            Assert.False(refreshResp.IsSuccessStatusCode);
        }

        async Task<(TRes Msg, HttpStatusCode Code)> IssueToken<TRes>(HttpClient client = null, TokenRequest request = null)
        {
            if (client == null)
            {
                client = _factory.CreateClient();
            }

            if (request == null)
            {
                request = new TokenRequest
                {
                    AuthCode = TestAuthCode,
                    ClientId = TestClientId,
                    CodeVerifier = TestCodeVerifier,
                    GrantType = "authorization_code"
                };    
            }
            
            var reqContent = request.ToUrlEncodedContent();
            
            var resp = await client.PostAsync("/token", reqContent);
            var respStr = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respStr);
            
            var res = JsonConvert.DeserializeObject<TRes>(respStr);

            return (res, resp.StatusCode);
        }
    }
}