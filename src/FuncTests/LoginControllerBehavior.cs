﻿using System;
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
        public async Task ShouldReturn404WhenSaveSuccessfulForAbsentSession()
        {
            //Arrange
            var sessionId = Guid.NewGuid().ToString("N");
            var db = await _dbFixture.CreateDbAsync();
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var resp = await api.Call(s => s.SuccessLogin(sessionId, new AuthorizedSubjectInfo()));

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
            var resp = await api.Call(s => s.FailLogin(sessionId, new LoginError()));

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
            var resp = await api.Call(s => s.SuccessLogin(sessionId, new AuthorizedSubjectInfo()));

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
            var resp = await api.Call(s => s.FailLogin(sessionId, new LoginError()));

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn409WhenSaveSuccessfulForAlreadyCompletedSession()
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
                        Expiry = DateTime.MaxValue,
                    }
                },
                SessionInitiations = new[]
                {
                    new SessionInitiationDb
                    {
                        RedirectUri = redirectUri,
                        SessionId = sessionId,
                        Scope = "no-mater-scope",
                        CompleteDt = DateTime.Now
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var authInfo = new AuthorizedSubjectInfo
            {
                Subject = "foo"
            };

            //Act
            var resp = await api.Call(s => s.SuccessLogin(sessionId, authInfo));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn409WhenSaveErrorForAlreadyCompletedSession()
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
                        RedirectUri = redirectUri,
                        SessionId = sessionId,
                        Scope = "no-mater-scope",
                        CompleteDt = DateTime.Now
                    }
                }
            };

            var db = await _dbFixture.CreateDbAsync(dataInitializer);
            var api = _testApi.Start(s => s.AddSingleton(db));

            var loginError = new LoginError
            {
                AuthError = AuthorizationRequestProcessingError.InvalidRequest
            };

            //Act
            var resp = await api.Call(s => s.FailLogin(sessionId, loginError));

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }
    }
}
