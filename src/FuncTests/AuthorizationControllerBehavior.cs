using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Db;
using MyLab.Db;
using Xunit;

namespace FuncTests
{
    public partial class AuthorizationControllerBehavior
    {
        [Theory]
        [InlineData("Unsupported resp type", "unsupported_response_type", "invalid", "valid", "http://host.net/cb", "openapi")]
        [InlineData("Null response type",  "invalid_request", null, "valid", "http://host.net/cb", "openapi")]
        [InlineData("Null client id", "invalid_request", "code", null, "http://host.net/cb", "openapi")]
        [InlineData("Null redirect uri", "invalid_request", "code", "valid", null, "openapi")]
        [InlineData("Null scope", "invalid_request", "code", "valid", "http://host.net/cb", null)]
        public async Task ShouldRedirectToErrPageWhen(string when, string expectedErrorCode, string responseType, string clientId, string redirectUri, string scope)
        {
            //Arrange
            var db = await _dbFixture.CreateDbAsync();

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                responseType, clientId, redirectUri, scope, null, null));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            _output.WriteLine("Error: " + query["error_description"]);

            //Assert
            Assert.Equal(redirectUri ?? TestConfigDefaultErrorEndpoint, locationLeftPart);
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(expectedErrorCode, query["error"]);
        }

        [Fact]
        public async Task ShouldFailedWhenWrongClient()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var dataInitializer = DataDbInitializer.Create(clientId, "http://host.net/cb", "testscope");

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", "wrong_client", "http://host.net/cb", "testscope", null, null));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal("http://host.net/cb", locationLeftPart);
            Assert.Equal("unauthorized_client", query?["error"]);
        }

        [Fact]
        public async Task ShouldFailedWhenWrongClientScope()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var dataInitializer = DataDbInitializer.Create(clientId, "http://host.net/cb", "testscope");

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, "http://host.net/cb", "wrong-scope", null, null));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal("http://host.net/cb", locationLeftPart);
            Assert.Equal("invalid_scope", query?["error"]);
        }

        [Fact]
        public async Task ShouldFailedWhenWrongRedirectUri()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var dataInitializer = DataDbInitializer.Create(clientId, "http://host.net/cb", "testscope");

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, "http://wrong.net/cb", "testscope", null, null));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigDefaultErrorEndpoint, locationLeftPart);
            Assert.Equal("invalid_request", query?["error"]);
        }

        [Fact]
        public async Task ShouldRedirectToExpectedPageWhenNoError()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var dataInitializer = DataDbInitializer.Create(clientId, "http://host.net/cb", "testscope");

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, "http://host.net/cb", "testscope", null, null));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigLoginEndpoint, locationLeftPart);
            Assert.NotNull(query?["login_id"]);
        }

        [Fact]
        public async Task ShouldUseCookieAndLoadExistSessionWithNewTokenSession()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var authCode = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var scopes = "testscope";

            var dataInitializer = CreateDataInitializerForCookieTest(clientId, loginSessId, tokenSessId, redirectUri, scopes, authCode);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, redirectUri, scopes, null, $"{LoginSessionCookieName.Name}={loginSessId}"));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            var respCode = query["code"];

            var newTokenSess = await db.DoOnce()
                .Tab<TokenSessionDb>()
                .Where(ts => ts.AuthCode == respCode)
                .FirstOrDefaultAsync();

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(redirectUri, locationLeftPart);
            Assert.NotNull(newTokenSess);
            Assert.NotEqual(tokenSessId, newTokenSess.Id);
            Assert.Equal(loginSessId, newTokenSess.LoginId);
            Assert.Equal(TokenSessionDbStatus.Ready, newTokenSess.Status);
        }

        [Fact]
        public async Task ShouldUseCookieAndIgnoreWhenSessionNotFound()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var loginSessId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var scopes = "testscope";

            var dataInitializer = DataDbInitializer.Create(clientId, redirectUri, scopes);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, redirectUri, scopes, null, $"{LoginSessionCookieName.Name}={loginSessId}"));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigLoginEndpoint, locationLeftPart);
            Assert.NotNull(query["login_id"]);
        }

        [Fact]
        public async Task ShouldUseCookieAndErrorWhenClientMismatch()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var client2Id = Guid.NewGuid().ToString("N");
            var loginSessId = Guid.NewGuid().ToString("N");
            var authCode = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var scopes = "testscope";

            var dataInitializer = CreateDataInitializerForCookieTest(clientId, loginSessId, redirectUri, scopes, authCode);

            dataInitializer.Clients = new[]
            {
                dataInitializer.Clients.First(),
                new ClientDb {Id = client2Id, Name = "bar", PasswordHash = TestTools.ClientPasswordHash}
            };

            dataInitializer.ClientScopes = new []
            {
                dataInitializer.ClientScopes.First(),
                new ClientAvailableScopeDb{ClientId = client2Id, Name = "testscope"}, 
            };

            dataInitializer.ClientRedirectUris = new[]
            {
                dataInitializer.ClientRedirectUris.First(),
                new ClientAvailableUriDb() {ClientId = client2Id, Uri = redirectUri},
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", client2Id, redirectUri, scopes, null, $"{LoginSessionCookieName.Name}={loginSessId}"));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigLoginEndpoint, locationLeftPart);
            Assert.NotNull(query["login_id"]);
        }

        [Fact]
        public async Task ShouldUseCookieAndIgnoreWhenMismatchRedirectUri()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var redirectUri2 = "http://host2.net/cb";
            var scopes = "testscope";

            var dataInitializer = CreateDataInitializerForCookieTest(clientId, loginSessId, tokenSessId, redirectUri, scopes);

            dataInitializer.ClientRedirectUris = new []
            {
                new ClientAvailableUriDb{ClientId = clientId, Uri = redirectUri},
                new ClientAvailableUriDb{ClientId = clientId, Uri = redirectUri2},
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, redirectUri2, scopes, null, $"{LoginSessionCookieName.Name}={loginSessId}"));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigLoginEndpoint, locationLeftPart);
            Assert.NotNull(query["login_id"]);
        }

        [Fact]
        public async Task ShouldUseCookieAndIgnoreWhenMismatchScopes()
        {
            //Arrange
            var clientId = Guid.NewGuid().ToString("N");
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var scopes = "testscope";
            var scopes2 = "testscope2";

            var dataInitializer = CreateDataInitializerForCookieTest(clientId, loginSessId, tokenSessId, redirectUri, scopes);

            dataInitializer.ClientScopes = new[]
            {
                dataInitializer.ClientScopes.First(),
                new ClientAvailableScopeDb{ClientId = clientId, Name = scopes2},
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);

            var oidc = _oidcTestApi.Start(
                s => s.AddSingleton(db)
            );

            //Act
            var resp = await oidc.Call(s => s.Authorization(
                "code", clientId, redirectUri, scopes2, null, $"{LoginSessionCookieName.Name}={loginSessId}"));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(TestConfigLoginEndpoint, locationLeftPart);
            Assert.NotNull(query["login_id"]);
        }
    }
}
