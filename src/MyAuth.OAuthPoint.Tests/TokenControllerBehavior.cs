using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    using static TestLoginRegistry;
    public class TokenControllerBehavior: IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;
        
        public TokenControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }
        
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
            var client = _factory.CreateClient();
            var request = new TokenRequest
            {
                AuthCode = authCode,
                ClientId = clientId,
                CodeVerifier = codeVerifier,
                GrantType = "authorization_code"
            };
            var reqContent = request.ToUrlEncodedContent();

            //Act
            var resp = await client.PostAsync("/token", reqContent);
            var respContent = await resp.Content.ReadAsStringAsync();

            var respError = JsonConvert.DeserializeObject<ErrorTokenResponse>(respContent);
            _output.WriteLine(respError.ErrorDescription);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            Assert.Equal(expectedCode, respError.ErrorCode);
        }
        
        [Fact]
        public async Task ShouldPassValidRequests()
        {
            //Arrange
            var client = _factory.CreateClient();
            var request = new TokenRequest
            {
                AuthCode = TestAuthCode,
                ClientId = TestClientId,
                CodeVerifier = TestCodeVerifier,
                GrantType = "authorization_code"
            };
            var reqContent = request.ToUrlEncodedContent();
            
            //Act
            var resp = await client.PostAsync("/token", reqContent);
            var respContent = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respContent);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldIssueValidToken()
        {
            var client = _factory.CreateClient();
            var request = new TokenRequest
            {
                AuthCode = TestAuthCode,
                ClientId = TestClientId,
                CodeVerifier = TestCodeVerifier,
                GrantType = "authorization_code"
            };
            var reqContent = request.ToUrlEncodedContent();
            
            //Act
            var resp = await client.PostAsync("/token", reqContent);
            var respContent = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respContent);

            if(!resp.IsSuccessStatusCode)
                throw new Exception("Wrong response code");
            
            var tokenResp = JsonConvert.DeserializeObject<SuccessTokenResponse>(respContent);

            //Assert
            Assert.Equal("bearer", tokenResp.TokenType);
            Assert.Equal(600, tokenResp.ExpiresIn);
            CheckRefreshToken(tokenResp.RefreshToken);
            CheckAccessToken(tokenResp.AccessToken);
        }

        private void CheckAccessToken(string tokenRespAccessToken)
        {
            int dot1Position = tokenRespAccessToken.IndexOf('.');
            int dot2Position = tokenRespAccessToken.IndexOf('.', dot1Position+1);
            
            if(dot1Position == -1 || 
               dot1Position == 0 || 
               dot1Position == tokenRespAccessToken.Length-1 ||
               dot2Position == -1 ||
               dot2Position == tokenRespAccessToken.Length-1 ||
               tokenRespAccessToken.IndexOf('.', dot2Position+1) != -1)
                throw new Exception("Wrong JWT token format");

            var header = GetHeader();
            
            Assert.Equal("JWT", header.Type); 
            Assert.Equal("HS256", header.Algorithm);
            
            CheckSign();

            var idToken = GetIdToken();
            
            Assert.NotNull(idToken);
            Assert.Equal("MyAuth.OAuthPoint", idToken.Issuer);
            Assert.Equal(TestClientId, idToken.Subject);
            Assert.True(DateTime.Now < idToken.GetExpirationTime());
            Assert.True(DateTime.Now < idToken.GetExpirationTime());
            Assert.Contains(idToken.Roles, s => TestRole == s);
            Assert.Contains(idToken.Climes, c => c.Name == TestClimeName && c.Value == TestClimeValue);

            IdentityToken GetIdToken()
            {
                var idTokenBase64 = tokenRespAccessToken.Substring(dot1Position + 1, dot2Position - dot1Position-1);
                var idTokenBin = WebEncoders.Base64UrlDecode(idTokenBase64);
                var idTokenStr = Encoding.UTF8.GetString(idTokenBin);
                return JsonConvert.DeserializeObject<IdentityToken>(idTokenStr);
            }
            
            void CheckSign()
            {
                var sign = tokenRespAccessToken.Substring(dot2Position + 1);
                
                var dataStr = tokenRespAccessToken.Remove(dot2Position) + "qwerty";
                var dataBin = Encoding.UTF8.GetBytes(dataStr);

                var hashAlg = SHA256.Create();
                var calcSignBin = hashAlg.ComputeHash(dataBin);
                var calcSignStr = WebEncoders.Base64UrlEncode(calcSignBin);
                
                Assert.Equal(calcSignStr, sign);
            }
                
            JwtHeader GetHeader()
            {
                var headerBase64 = tokenRespAccessToken.Remove(dot1Position);
                var headerBin = WebEncoders.Base64UrlDecode(headerBase64);
                var headerStr = Encoding.UTF8.GetString(headerBin);
                
                return JsonConvert.DeserializeObject<JwtHeader>(headerStr);
            }
        }

        private void CheckRefreshToken(string tokenRespRefreshToken)
        {
            var binGuid = WebEncoders.Base64UrlDecode(tokenRespRefreshToken);
            var strGuid = Encoding.UTF8.GetString(binGuid);
            
            Assert.True(Guid.TryParse(strGuid, out var resGuid));
        }
    }
}