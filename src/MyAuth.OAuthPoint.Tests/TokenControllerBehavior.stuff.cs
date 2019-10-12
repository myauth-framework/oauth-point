using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using MyAuth.OAuthPoint.Models;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public partial class TokenControllerBehavior : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;
        private readonly TokensTestTools _tt;

        public TokenControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _tt = new TokensTestTools(_factory, _output);

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        private void CheckAccessToken(string tokenRespAccessToken)
        {
            var sth = new JwtSecurityTokenHandler();
            var at = sth.ReadJwtToken(tokenRespAccessToken);


            Assert.Equal("JWT", at.Header.Typ);
            Assert.Equal("HS256", at.Header.Alg);

            sth.ValidateToken(tokenRespAccessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = TestTokenIssuingOptions.Options.Issuer,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyqwertyqwerty")),
                ValidateAudience = true,
                ValidAudience = TestLoginRegistry.TestAudience
            }, out _);

            Assert.Equal(TestLoginRegistry.TestUserId, at.Subject);
            Assert.Contains(at.Claims, c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == TestLoginRegistry.TestRole);
            Assert.Contains(at.Claims, c => c.Type == TestLoginRegistry.TestClimeName && c.Value == TestLoginRegistry.TestClimeValue);
        }   

        private void CheckRefreshToken(string tokenRespRefreshToken)
        {
            var binGuid = WebEncoders.Base64UrlDecode(tokenRespRefreshToken);
            var strGuid = Encoding.UTF8.GetString(binGuid);

            Assert.True(Guid.TryParse(strGuid, out _));
        }

        async Task<(TRes Msg, HttpStatusCode Code)> IssueToken<TRes>
            (HttpClient client = null, TokenRequest request = null) =>
                await _tt.IssueToken<TRes>(client, request);

        async Task<(TRes Msg, HttpStatusCode Code)> RefreshToken<TRes>
            (string refreshToken, HttpClient client = null) =>
                await _tt.RefreshToken<TRes>(refreshToken, client);
    }
}