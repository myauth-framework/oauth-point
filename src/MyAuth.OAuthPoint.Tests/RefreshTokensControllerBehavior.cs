using System;
using System.Net;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class RefreshTokensControllerBehavior : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private TokensTestTools _tt;

        /// <summary>
        /// Initializes a new instance of <see cref="RefreshTokensControllerBehavior"/>
        /// </summary>
        public RefreshTokensControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _tt = new TokensTestTools(_factory, output);
        }
        
        [Fact]
        public async Task ShouldRevokeRefreshToken()
        {
            //Arrange
            var client = _factory.CreateClient();
            var issueTokenResponse = await _tt.IssueToken<SuccessTokenResponse>(client);
            
            
            //Act
            var revokeResp = await client.DeleteAsync("/refresh-tokens/" + issueTokenResponse.Msg.RefreshToken);
            if (!revokeResp.IsSuccessStatusCode)
                throw new Exception("Can't revoke refresh token");

            var refreshResp = await _tt.RefreshToken<ErrorTokenResponse>(issueTokenResponse.Msg.RefreshToken, client);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, refreshResp.Code);
        }
    }
}