using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
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
            int dot1Position = tokenRespAccessToken.IndexOf('.');
            int dot2Position = tokenRespAccessToken.IndexOf('.', dot1Position + 1);

            if (dot1Position == -1 ||
                dot1Position == 0 ||
                dot1Position == tokenRespAccessToken.Length - 1 ||
                dot2Position == -1 ||
                dot2Position == tokenRespAccessToken.Length - 1 ||
                tokenRespAccessToken.IndexOf('.', dot2Position + 1) != -1)
                throw new Exception("Wrong JWT token format");

            var header = GetHeader();

            Assert.Equal("JWT", header.Type);
            Assert.Equal("HS256", header.Algorithm);

            CheckSign();

            var idToken = GetIdToken();

            Assert.NotNull(idToken);
            Assert.Equal(TestTokenIssuingOptions.Options.Issuer, idToken.Issuer);
            Assert.Equal(TestLoginRegistry.TestClientId, idToken.Subject);
            Assert.True(DateTime.Now < idToken.GetExpirationTime());
            Assert.True(DateTime.Now < idToken.GetExpirationTime());
            Assert.Contains(idToken.Roles, s => TestLoginRegistry.TestRole == s);
            Assert.Contains(idToken.Climes,
                c => c.Name == TestLoginRegistry.TestClimeName && c.Value == TestLoginRegistry.TestClimeValue);

            IdentityToken GetIdToken()
            {
                var idTokenBase64 = tokenRespAccessToken.Substring(dot1Position + 1, dot2Position - dot1Position - 1);
                var idTokenBin = WebEncoders.Base64UrlDecode(idTokenBase64);
                var idTokenStr = Encoding.UTF8.GetString(idTokenBin);
                return JsonConvert.DeserializeObject<IdentityToken>(idTokenStr);
            }

            void CheckSign()
            {
                var sign = tokenRespAccessToken.Substring(dot2Position + 1);

                var dataStr = tokenRespAccessToken.Remove(dot2Position);
                var dataBin = Encoding.UTF8.GetBytes(dataStr);

                var binSecret = Encoding.UTF8.GetBytes(TestTokenIssuingOptions.Options.Secret);
                var hashAlg = new HMACSHA256(binSecret);
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

        async Task<(TRes Msg, HttpStatusCode Code)> IssueToken<TRes>
            (HttpClient client = null, TokenRequest request = null) =>
                await _tt.IssueToken<TRes>(client, request);

        async Task<(TRes Msg, HttpStatusCode Code)> RefreshToken<TRes>
            (string refreshToken, HttpClient client = null) =>
                await _tt.RefreshToken<TRes>(refreshToken, client);
    }
}