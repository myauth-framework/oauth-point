using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using MyLab.Db;
using MyLab.DbTest;
using Xunit;

namespace FuncTests
{
    public partial class MyAuthPointBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        [Theory]
        [InlineData("unsupported_response_type", "invalid", "valid", "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", null, "valid", "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", "code", null, "http://host.net/cb", "openapi")]
        [InlineData("invalid_request", "code", "valid", null, "openapi")]
        [InlineData("invalid_request", "code", "valid", "http://host.net/cb", null)]
        public async Task ShouldRedirectToErrPageWhenError(string expectedErrorCode, string responseType, string clientId, string redirectUri, string scope)
        {
            //Arrange
            var dataInitializer = new TestDataDbInitializer();
            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton<IDbManager>(db)
            );

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
            var dataInitializer = new TestDataDbInitializer();
            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton<IDbManager>(db)
            );

            Uri? newLocationUrl = null;
            NameValueCollection query = null;

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", "valid", "valid", "openid", null));

            if (resp.StatusCode == HttpStatusCode.Redirect)
            {
                newLocationUrl = resp.ResponseMessage.Headers.Location;

                if(newLocationUrl != null)
                    query = HttpUtility.ParseQueryString(newLocationUrl.Query);
            }

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal("http://host.net/login", newLocationUrl.GetLeftPart(UriPartial.Path));
            Assert.NotNull(query?["login_id"]);
        }
    }

    public class TestDataDbInitializer : ITestDbInitializer
    {
        public async Task InitializeAsync(DataConnection dataConnection)
        {
        }
    }
}
