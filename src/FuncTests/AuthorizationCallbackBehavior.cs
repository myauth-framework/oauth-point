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
    public class AuthorizationCallbackBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IApiServiceV1> _testApi;

        private const string TestConfigLoginEndpoint = "http://host.net/login";
        private const string TestConfigDefaultErrorEndpoint = "http://host.net/error";

        public AuthorizationCallbackBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
        {
            _output = output;
            _dbFixture = dbFixture;

            dbFixture.Output = output;

            _testApi = new TestApi<Startup, IApiServiceV1>
            {
                Output = output,
                ServiceOverrider = srv =>
                {
                    srv.Configure<AuthOptions>(opt =>
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
            var sessionId = Guid.NewGuid().ToString("N");
            

            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(sessionId));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404IfSessionExpired()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo" } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId, 
                        ClientId = clientId,
                        Expiry = DateTime.MinValue
                    }
                },
                SessionInitiations = new[]
                {
                    new SessionInitiationDb
                    {
                        RedirectUri = redirectUri,
                        SessionId = sessionId,
                        Scope = "no-mater-scope"
                    }
                }
            };
            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(sessionId));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldRedirectToErrorWhenWasLoginError()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";

            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo" } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId, 
                        ClientId = clientId,
                        Expiry = DateTime.MaxValue
                    }
                },
                SessionInitiations = new[]
                {
                    new SessionInitiationDb
                    {
                        ErrorCode = AuthorizationRequestProcessingError.InvalidRequestObject,
                        ErrorDesription = "error",
                        RedirectUri = redirectUri,
                        SessionId = sessionId,
                        Scope = "no-mater-scope",
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(sessionId));

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
            var sessionId = Guid.NewGuid().ToString("N");
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
                        Name = "foo"
                    }
                },
                LoginSessions= new []
                {
                    new LoginSessionDb
                    {
                        Id = sessionId,
                        ClientId = clientId,
                        LoginDt = DateTime.Now,
                        Expiry = DateTime.MaxValue
                    }
                },
                SessionInitiations = new[]
                {
                    new SessionInitiationDb
                    {
                        RedirectUri = redirectUri,
                        AuthorizationCode = authCode,
                        CompleteDt = DateTime.Now,
                        State = state,
                        SessionId = sessionId,
                        Scope = "no-mater-scope"
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(sessionId));

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
            var sessionId = Guid.NewGuid().ToString("N");
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
                        Name = "foo"
                    }
                },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId,
                        ClientId = clientId,
                        LoginDt = DateTime.Now,
                        Expiry = DateTime.MaxValue
                    }
                },
                SessionInitiations = new[]
                {
                    new SessionInitiationDb
                    {
                        RedirectUri = redirectUri,
                        AuthorizationCode = authCode,
                        CompleteDt = DateTime.Now,
                        State = state,
                        SessionId = sessionId,
                        Scope = "no-mater-scope"
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.CallbackLogin(sessionId));

            var cookiesWasFound = resp.ResponseMessage.Headers.TryGetValues("Set-Cookie", out var values);
            var foundLoginCookies = values?.FirstOrDefault(c => c.StartsWith(LoginSessionCookieName.Name + "="));

            //Assert
            Assert.True(cookiesWasFound);
            Assert.NotNull(foundLoginCookies);
            Assert.StartsWith($"{LoginSessionCookieName.Name}={sessionId}", foundLoginCookies);
        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }
    }
}
