using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Client.Models;
using MyAuth.OAuthPoint.Db;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class LoginControllerBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IApiServiceV1> _testApi;

        private const string TestConfigLoginEndpoint = "http://host.net/login";
        private const string TestConfigDefaultErrorEndpoint = "http://host.net/error";

        public LoginControllerBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
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
        public async Task ShouldReturn404WhenSaveSuccessfulForAbsentSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.SuccessLogin(sessionId, new LoginSuccessRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForAbsentSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.FailLogin(sessionId, new LoginErrorRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveSuccessfulForExpiredSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithExpiredSession(sessionId);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.SuccessLogin(sessionId, new LoginSuccessRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForExpiredSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithExpiredSession(sessionId);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.FailLogin(sessionId, new LoginErrorRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveSuccessfulForAlreadyCompletedSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId,
                        ClientId = clientId,
                        Expiry = DateTime.MaxValue,
                        RedirectUri = redirectUri,
                        Scope = "no-mater-scope",
                        CompletedDt = DateTime.Now
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var authInfo = new LoginSuccessRequest
            {
                Subject = "foo"
            };

            //Act
            var resp = await api.Call(s => s.SuccessLogin(sessionId, authInfo));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForAlreadyCompletedSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = sessionId,
                        ClientId = clientId,
                        Expiry = DateTime.MaxValue,
                        RedirectUri = redirectUri,
                        Scope = "no-mater-scope",
                        CompletedDt = DateTime.Now
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var loginError = new LoginErrorRequest
            {
                AuthError = AuthorizationRequestProcessingError.InvalidRequest
            };

            //Act
            var resp = await api.Call(s => s.FailLogin(sessionId, loginError));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }
    }
}
