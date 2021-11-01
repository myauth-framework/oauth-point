using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Client.Models;
using MyAuth.OAuthPoint.Db;
using MyLab.ApiClient.Test;
using MyLab.Db;
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
            var loginSessId = Guid.NewGuid().ToString("N");
            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, new LoginSuccessRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForAbsentSession()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.FailLogin(loginSessId, new LoginErrorRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveSuccessfulForExpiredSession()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithExpiredSession(loginSessId,tokenSessId);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, new LoginSuccessRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForExpiredSession()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithExpiredSession(loginSessId, tokenSessId);

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.FailLogin(loginSessId, new LoginErrorRequest()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveSuccessfulForAlreadyCompletedSession()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue,
                        LoginExpiry = DateTime.Now.AddSeconds(10),
                        Status = LoginSessionDbStatus.Started
                    }
                },
                TokenSessions = new []
                {
                    new TokenSessionDb
                    {
                        Id = tokenSessId,
                        LoginId = loginSessId,
                        ClientId = clientId,
                        RedirectUri = redirectUri,
                        Scope = "no-mater-scope",
                        Status = TokenSessionDbStatus.Started
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
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, authInfo));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn404WhenSaveErrorForAlreadyCompletedSession()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue,
                        LoginExpiry = DateTime.Now.AddSeconds(10),
                        Status = LoginSessionDbStatus.Started
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
                        Scope = "no-mater-scope",
                        Status = TokenSessionDbStatus.Started
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
            var resp = await api.Call(s => s.FailLogin(loginSessId, loginError));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldCompleteSuccessful()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue,
                        LoginExpiry = DateTime.Now.AddSeconds(10),
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
                        Scope = "no-mater-scope"
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
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, authInfo));

            var actualSess = await db.DoOnce().Tab<TokenSessionDb>()
                .Where(s => s.Id == tokenSessId)
                .Select(s => new
                {
                    LoginSessStatus = s.Login.Status,
                    TokenSessStatus = s.Status,
                    s.Login.SubjectId,
                    s.AuthCode,
                    s.AuthCodeExpiry,
                    s.ErrorCode
                }).FirstOrDefaultAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.NotNull(actualSess);
            Assert.Equal(LoginSessionDbStatus.Started, actualSess.LoginSessStatus);
            Assert.Equal(TokenSessionDbStatus.Started, actualSess.TokenSessStatus);
            Assert.Equal("foo", actualSess.SubjectId);
            Assert.NotNull(actualSess.AuthCode);
            Assert.True(actualSess.AuthCodeExpiry > DateTime.Now);
            Assert.Equal(MyAuth.OAuthPoint.Models.AuthorizationRequestProcessingError.Undefined, actualSess.ErrorCode);
        }

        [Fact]
        public async Task ShouldCompleteWithError()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var clientId = Guid.NewGuid().ToString("N");
            var redirectUri = "http://host.net/cb";
            var dataInitializer = new DataDbInitializer
            {
                Clients = new[] { new ClientDb { Id = clientId, Name = "foo", PasswordHash = TestTools.ClientPasswordHash } },
                LoginSessions = new[]
                {
                    new LoginSessionDb
                    {
                        Id = loginSessId,
                        Expiry = DateTime.MaxValue,
                        LoginExpiry = DateTime.Now.AddSeconds(10)
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
                        Scope = "no-mater-scope"
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));
            
            var errReq = new LoginErrorRequest
            {
                AuthError = AuthorizationRequestProcessingError.AccessDenied,
                Description = "foo-desc"
            };

            //Act
            var resp = await api.Call(s => s.FailLogin(loginSessId, errReq));

            var actualSess = await db.DoOnce().Tab<TokenSessionDb>()
                .Where(s => s.Id == tokenSessId)
                .Select(s => new
                {
                    LoginSessStatus = s.Login.Status,
                    TokenSessStatus = s.Status,
                    s.Login.SubjectId,
                    s.AuthCode,
                    s.AuthCodeExpiry,
                    s.ErrorCode
                }).FirstOrDefaultAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.NotNull(actualSess);
            Assert.Equal(LoginSessionDbStatus.Failed, actualSess.LoginSessStatus);
            Assert.Equal(TokenSessionDbStatus.Failed, actualSess.TokenSessStatus);
            Assert.Null(actualSess.SubjectId);
            Assert.Null(actualSess.AuthCode);
            Assert.Null(actualSess.AuthCodeExpiry);
            Assert.Equal(MyAuth.OAuthPoint.Models.AuthorizationRequestProcessingError.AccessDenied, actualSess.ErrorCode);
        }

        [Fact]
        public async Task ShouldReturn400WhenSubjectNotSpecified()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithSession(loginSessId, tokenSessId, DateTime.Now.AddDays(1));

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var succReq = new LoginSuccessRequest
            {
                Subject = null
            };

            //Act
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, succReq));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

        }

        [Fact]
        public async Task ShouldNotFailWhenNullValueClaim()
        {
            //Arrange
            var loginSessId = Guid.NewGuid().ToString("N");
            var tokenSessId = Guid.NewGuid().ToString("N");
            var dataInitializer = TestTools.CreateDataIniterWithSession(loginSessId, tokenSessId,DateTime.Now.AddDays(1));

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var succReq = new LoginSuccessRequest
            {
                Subject = "foo",
                AccessClaims = new ClaimsCollection(){ {"key", null} },
                IdentityScopes = new []
                {
                    new ScopeClaims
                    {
                        Id = "id",
                        Claims = new ClaimsCollection(){ {"key", null} }
                    }
                }
            };

            //Act
            var resp = await api.Call(s => s.SuccessLogin(loginSessId, succReq));

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }
    }
}
