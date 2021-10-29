using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Db;
using MyAuth.OAuthPoint.Models;
using MyLab.ApiClient.Test;
using MyLab.Db;
using MyLab.DbTest;
using Xunit;
using Xunit.Abstractions;
using LoginSessionCookieName = MyAuth.OAuthPoint.Client.LoginSessionCookieName;

namespace FuncTests
{
    public class AuthorizationCallbackControllerBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IApiServiceV1> _testApi;

        private const string TestConfigLoginEndpoint = "http://host.net/login";
        private const string TestConfigDefaultErrorEndpoint = "http://host.net/error";

        public AuthorizationCallbackControllerBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
        {
            _output = output;
            _dbFixture = dbFixture;

            dbFixture.Output = output;

            _testApi = new TestApi<Startup, IApiServiceV1>
            {
                Output = output,
                ServiceOverrider = srv =>
                {
                    srv.Configure<AuthEndpointsOptions>(opt =>
                    {
                        opt.DefaultErrorEndpoint = TestConfigDefaultErrorEndpoint;
                        opt.LoginEndpoint = TestConfigLoginEndpoint;
                    });

                    srv.AddLogging(lb =>
                    {
                        lb.AddXUnit(_output);
                        lb.AddFilter(level => true);
                    });
                }
            };
        }

        [Fact]
        public async Task ShouldReturn404IfSessionNotFound()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");

            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(loginSessId, clientId));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404IfSessionExpired()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithExpiredSession(loginSessId, tokenSessId);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(loginSessId, clientId));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldRedirectToErrorWhenWasLoginError()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb
                {
                    Id = clientId, 
                    Name = "foo", 
                    PasswordHash = TestTools.ClientPasswordHash
                } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId, 
                        Expiry = DateTime.MaxValue,
                    }
                },
                TokenSessions = new []
                {
                    new TokenSessionDb
                    {
                        Id = tokenSessId,
                        LoginId = loginSessId,
                        ClientId = clientId,
                        ErrorCode = AuthorizationRequestProcessingError.InvalidRequestObject,
                        ErrorDesc = "error",
                        RedirectUri = redirectUri,
                        Scope = "no-mater-scope",
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(loginSessId, clientId));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(redirectUri, locationLeftPart);
            Assert.Equal("invalid_request_object", query["error"]);
            Assert.Equal("error", query["error_description"]);
        }

        [Fact]
        public async Task ShouldRedirectToCallbackWhenLoginSuccess()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var authCode = Guid.NewGuid().ToString("N");
            var state = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            var dataInitializer = new DataDbInitializer
            {
                Clients = new[]
                {
                    new ClientDb
                    {
                        Id = clientId, 
                        Name = "foo",
                        PasswordHash = TestTools.ClientPasswordHash
                    }
                },
                LoginSessions= new []
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue,
                    }
                },
                TokenSessions = new[]
                {
                    new TokenSessionDb
                    {
                        Id = tokenSessId,
                        LoginId = loginSessId,
                        ClientId = clientId,
                        RedirectUri = redirectUri,
                        AuthCode = authCode,
                        State = state,
                        Scope = "no-mater-scope"
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(loginSessId, clientId));

            TestTools.TryExtractRedirect(resp, out var locationLeftPart, out var query);

            //Assert
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.Equal(redirectUri, locationLeftPart);
            Assert.Equal(authCode, query["code"]);
            Assert.Equal(state, query["state"]);
        }

        [Fact]
        public async Task ShouldSetLoginCookieWhenLoginSuccess()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var authCode = Guid.NewGuid().ToString("N");
            var state = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            var dataInitializer = new DataDbInitializer
            {
                Clients = new[]
                {
                    new ClientDb
                    {
                        Id = clientId,
                        Name = "foo",
                        PasswordHash = TestTools.ClientPasswordHash
                    }
                },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue
                    }
                },
                TokenSessions = new[]
                {
                    new TokenSessionDb
                    {
                        Id = tokenSessId,
                        LoginId = loginSessId,
                        ClientId = clientId,
                        RedirectUri = redirectUri,
                        AuthCode= authCode,
                        State = state,
                        Scope = "no-mater-scope"
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(loginSessId, clientId));

            var cookiesWasFound = resp.ResponseMessage.Headers.TryGetValues("Set-Cookie", out var values);
            var foundLoginCookies = values?.FirstOrDefault(c => c.StartsWith(LoginSessionCookieName.Name + "="));

            //Assert
            Assert.True(cookiesWasFound);
            Assert.NotNull(foundLoginCookies);
            Assert.StartsWith($"{LoginSessionCookieName.Name}={loginSessId}", foundLoginCookies);
        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }
    }
}
