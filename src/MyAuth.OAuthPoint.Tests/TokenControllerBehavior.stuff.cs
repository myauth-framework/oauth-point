using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using MyAuth.Common;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    public partial class TokenControllerBehavior : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;
        private TokensTestTools _tt;

        public TokenControllerBehavior(
            TestWebApplicationFactory factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _tt = new TokensTestTools(_factory, _output);
        }

        private void CheckAccessToken(string tokenRespAccessToken)
        {
            var at = AccessToken.Deserialize(tokenRespAccessToken);

            Assert.Equal("JWT", at.Header.Type);
            Assert.Equal("HS256", at.Header.Algorithm);
            Assert.True(at.VerifySignature("qwerty"));
            Assert.Equal(TestTokenIssuingOptions.Options.Issuer, at.IdToken.Issuer);
            Assert.Equal(TestLoginRegistry.TestUserId, at.IdToken.Subject);
            Assert.True(DateTime.Now < at.IdToken.GetExpirationTime());
            Assert.True(DateTime.Now < at.IdToken.GetExpirationTime());
            Assert.Contains(at.IdToken.Roles, s => TestLoginRegistry.TestRole == s);
            Assert.Contains(at.IdToken.Climes,
                c => c.Name == TestLoginRegistry.TestClimeName && c.Value == TestLoginRegistry.TestClimeValue);
        }

        private void CheckRefreshToken(string tokenRespRefreshToken)
        {
            var binGuid = WebEncoders.Base64UrlDecode(tokenRespRefreshToken);
            var strGuid = Encoding.UTF8.GetString(binGuid);

            Assert.True(Guid.TryParse(strGuid, out var resGuid));
        }

        async Task<(TRes Msg, HttpStatusCode Code)> IssueToken<TRes>
            (HttpClient client = null, TokenRequest request = null) =>
                await _tt.IssueToken<TRes>(client, request);

        async Task<(TRes Msg, HttpStatusCode Code)> RefreshToken<TRes>
            (string refreshToken, HttpClient client = null) =>
                await _tt.RefreshToken<TRes>(refreshToken, client);
    }
}