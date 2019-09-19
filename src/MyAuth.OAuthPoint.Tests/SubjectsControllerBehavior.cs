using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public class SubjectsControllerBehavior : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;
        private TokensTestTools _tt;

        /// <summary>
        /// Initializes a new instance of <see cref="SubjectsControllerBehavior"/>
        /// </summary>
        public SubjectsControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _tt = new TokensTestTools(_factory, output);
        }
        
        [Fact]
        public async Task ShouldRevokeRefreshTokensBySubject()
        {
            //Arrange
            var client = _factory.CreateClient();
            var issueTokenResponse = await _tt.IssueToken<SuccessTokenResponse>(client);
            
            
            //Act
            var revokeResp = await client.DeleteAsync(
                "/subjects/" + 
                TestLoginRegistry.TestUserId + 
                "/refresh-tokens");

            if (!revokeResp.IsSuccessStatusCode)
            {
                _output.WriteLine("Response code: "+ revokeResp.StatusCode);
                throw new Exception("Can't revoke refresh token");
            }

            var refreshResp = await _tt.RefreshToken<ErrorTokenResponse>(issueTokenResponse.Msg.RefreshToken, client);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, refreshResp.Code);
        }
    }
}