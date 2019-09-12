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
    public partial class TokenControllerBehavior
    {
        [Fact]
        public async Task ShouldRefreshToken()
        {
            //Arrange
            var client = _factory.CreateClient();
            var issueTokenResponse = await IssueToken<SuccessTokenResponse>(client);

            //Act
            var refreshTokenResponse = await RefreshToken<SuccessTokenResponse>(issueTokenResponse.Msg.RefreshToken, client);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, refreshTokenResponse.Code);
            Assert.Equal("bearer", refreshTokenResponse.Msg.TokenType);
            Assert.Equal(600, refreshTokenResponse.Msg.ExpiresIn);
            CheckRefreshToken(refreshTokenResponse.Msg.RefreshToken);
            CheckAccessToken(refreshTokenResponse.Msg.AccessToken);
        }

        [Fact]
        public async Task ShouldNotRefreshTokenForInvalidRefreshToken()
        {
            //Arrange
            var invalidRefreshToken = Tools.RefreshToken.Generate(1);
            
            //Act
            var refreshTokenResponse = await RefreshToken<ErrorTokenResponse>(invalidRefreshToken.Body);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, refreshTokenResponse.Code);
        }
    }
}