using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        [InlineData("Wrong authCode", "foo", "bar", "baz")]
        [InlineData("Wrong clientId", TestAuthCode, "bar", "baz")]
        [InlineData("Wrong code verifier", TestAuthCode, TestClientId, "baz")]
        [InlineData("Empty code verifier", TestAuthCode, TestClientId, "")]
        public async Task ShouldFailInvalidRequests(string desc, string authCode, string clientId, string codeVerifier)
        {
            //Arrange
            var client = _factory.CreateClient();
            var query = $"code={authCode}&client_id={clientId}&code_verifier={codeVerifier}";

            //Act
            var resp = await client.GetAsync("/token?" + query);
            var respContent = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respContent);
            
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
        
        [Fact]
        public async Task ShouldPassValidRequests()
        {
            //Arrange
            var client = _factory.CreateClient();
            var query = $"code={TestAuthCode}&client_id={TestClientId}&code_verifier={TestCodeVerifier}";

            //Act
            var resp = await client.GetAsync("/token?" + query);
            var respContent = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respContent);
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}