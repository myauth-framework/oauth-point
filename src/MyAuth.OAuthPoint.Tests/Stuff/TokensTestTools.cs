using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyAuth.OAuthPoint.Models;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace MyAuth.OAuthPoint.Tests
{
    class TokensTestTools
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="TokensTestTools"/>
        /// </summary>
        public TokensTestTools(TestWebApplicationFactory factory, ITestOutputHelper _output)
        {
            _factory = factory;
            this._output = _output;
        }
        public async Task<(TRes Msg, HttpStatusCode Code)> IssueToken<TRes>(HttpClient client = null, TokenRequest request = null)
        {
            if (client == null)
            {
                client = _factory.CreateClient();
            }

            if (request == null)
            {
                request = new TokenRequest
                {
                    AuthCode = TestLoginRegistry.TestAuthCode,
                    ClientId = TestLoginRegistry.TestClientId,
                    CodeVerifier = TestLoginRegistry.TestCodeVerifier,
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

        public async Task<(TRes Msg, HttpStatusCode Code)> RefreshToken<TRes>(string refreshToken, HttpClient client = null)
        {
            if (client == null)
            {
                client = _factory.CreateClient();
            }
            
            var request = new TokenRequest
            {
                RefreshToken = refreshToken,
                ClientId = TestLoginRegistry.TestClientId,
                CodeVerifier = TestLoginRegistry.TestCodeVerifier,
                GrantType = "refresh_token"
            };
            var reqContent = request.ToUrlEncodedContent();
            
            //Act
            var refreshResp = await client.PostAsync("/token", reqContent);
            var respStr = await refreshResp.Content.ReadAsStringAsync();

            _output.WriteLine(respStr);
            
            var res = JsonConvert.DeserializeObject<TRes>(respStr);

            return (res, refreshResp.StatusCode);
        }
    }
}