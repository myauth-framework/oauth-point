using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyLab.ApiClient.Test;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class MyAuthPointBehavior : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TestApi<Startup, IOidcServiceContractV1> _oidcTestApi;

        public MyAuthPointBehavior(ITestOutputHelper output)
        {
            _output = output;

            _oidcTestApi = new TestApi<Startup, IOidcServiceContractV1>()
            {
                Output = output,
                ServiceOverrider = srv =>
                {
                    srv.Configure<AuthOptions>(opt =>
                    {
                        opt.DefaultErrorEndpoint = "http://host.net/error";
                        opt.LoginEndpoint = "http://host.net/login";
                    });

                    srv.AddLogging(lb =>
                    {
                        lb.AddXUnit(_output);
                        lb.AddFilter(level => true);
                    });
                }
            };
        }

        [Theory]
        [InlineData("unsupported_response_type", "invalid", "valid", "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", null, "valid", "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", "code", null, "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", "code", "valid", null, "openapi")]
        [InlineData("invalid_request", "code", "valid", "http://host.net/cb", null)]
        public async Task ShouldRedirectToErrPageWhenError(string expectedErrorCode, string responseType, string clientId, string redirectUri, string scope)
        {
            //Arrange
            var oidc = _oidcTestApi.Start();

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                responseType, clientId, redirectUri, scope, null));

            var newLocationUrl = resp.ResponseMessage.Headers.Location;
            var query = HttpUtility.ParseQueryString(newLocationUrl.Query);

            _output.WriteLine("Error: " + query["error_description"]);

            //Assert
            Assert.Equal(redirectUri ?? "http://host.net/error", newLocationUrl.GetLeftPart(UriPartial.Path));
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(expectedErrorCode, query["error"]);
        }

        [Fact]
        public async Task ShouldRedirectToExpectedPageWhenNoError()
        {
            //Arrange
            var oidc = _oidcTestApi.Start();

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", "valid", "valid", "openid", null));

            var newLocationUrl = resp.ResponseMessage.Headers.Location;
            var query = HttpUtility.ParseQueryString(newLocationUrl.Query);

            //Assert
            Assert.Equal("http://local.loc/login", newLocationUrl.GetLeftPart(UriPartial.Path));
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.NotNull(query["login_id"]);
        }

        public void Dispose()
        {
            _oidcTestApi?.Dispose();
        }
    }
}
