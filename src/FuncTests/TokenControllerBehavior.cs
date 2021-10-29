using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Client.Models;
using MyAuth.OAuthPoint.Db;
using MyLab.ApiClient.Test;
using MyLab.Db;
using MyLab.DbTest;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using ErrorTokenResponse = MyAuth.OAuthPoint.Client.Models.ErrorTokenResponse;
using TokenRequest = MyAuth.OAuthPoint.Client.Models.TokenRequest;

namespace FuncTests
{
    public class TokenControllerBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IOidcServiceContractV1> _testApi;

        public TokenControllerBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
        {
            _output = output;
            _dbFixture = dbFixture;

            dbFixture.Output = output;

            _testApi = new TestApi<Startup, IOidcServiceContractV1>
            {
                Output = output,
                ServiceOverrider = srv =>
                {
                    srv.Configure<AuthStoringOptions>(opt =>
                    {
                        opt.ClientPasswordSalt = TestTools.PasswordSalt;
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
        [MemberData(nameof(GetInvalidTokenRequests))]
        public async Task ShouldFailIfWrongRequestFormat(string f, TokenRequest request, string authHeaderValue, TokenRequestProcessingError expectedError)
        {
            //Arrange
            var db = await _dbFixture.CreateDbAsync(new SessionRegistrar());
            var api = _testApi.Start(s => s.AddSingleton(db));

            //Act
            var res = await api.Call(s => s.Token(request, authHeaderValue));

            var respStrContent = await res.ResponseMessage.Content.ReadAsStringAsync();
            ErrorTokenResponse error = null;

            try
            {
                error = JsonConvert.DeserializeObject<ErrorTokenResponse>(respStrContent);
            }
            catch (Exception e)
            {
                _output.WriteLine(e.Message);
            }

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            Assert.NotNull(error);
            Assert.Equal(expectedError, error.AuthError);
        }

        public static IEnumerable<object[]> GetInvalidTokenRequests()
        {
            string rightBasicAuthHeader = "Basic cmlnaHQtY2xpZW50OmJhcg==";
            string wrongBasicAuthHeader = "Basic Zm9vOmZvbw==";

            return new[]
            {
                new object[] {"Unsup. auth type", new TokenRequest(), "Bearer bla-bla", TokenRequestProcessingError.InvalidClient},
                new object[] {"Malformed auth", new TokenRequest(), "Basic", TokenRequestProcessingError.InvalidClient},
                new object[] 
                {
                    "Wrong login/pass", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        Code = "right-code",
                        GrantType = "authorization_code"
                    }, 
                    wrongBasicAuthHeader, TokenRequestProcessingError.InvalidRequest},
                new object[]
                {
                    "Wrong ClientId", new TokenRequest
                    {
                        ClientId = "wrong-client",
                        RedirectUri = "http://right.ru/cb",
                        Code = "right-code",
                        GrantType = "authorization_code"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidRequest
                },
                new object[]
                {
                    "Wrong RedirectUri", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://wrong.ru/cb",
                        Code = "right-code",
                        GrantType = "authorization_code"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidGrant
                },
                new object[]
                {
                    "Wrong Code", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        Code = "wrong-code",
                        GrantType = "authorization_code"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidGrant
                },
                new object[]
                {
                    "Wrong GrantType", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        Code = "right-code",
                        GrantType = "wrong"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.UnsupportedGrantType
                },
                new object[]
                {
                    "Empty refresh token", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        GrantType = "refresh_token"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidRequest
                },
                new object[]
                {
                    "Wrong refresh token", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        RefreshToken = "wrong",
                        GrantType = "refresh_token"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidGrant
                },
                new object[]
                {
                    "Wrong refr token and client", new TokenRequest
                    {
                        ClientId = "right-client",
                        RedirectUri = "http://right.ru/cb",
                        RefreshToken = "right-login-sess-id-2",
                        GrantType = "refresh_token"
                    },
                    rightBasicAuthHeader, TokenRequestProcessingError.InvalidGrant
                }
            };
        }

        public void Dispose()
        {
            _testApi?.Dispose();
        }

        class SessionRegistrar : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.Tab<ClientDb>().InsertAsync(() => new ClientDb
                {
                    Id = "right-client",
                    Name = "test-client",
                    Enabled = MySqlBool.True,
                    PasswordHash = TestTools.PasswordHashCalculator.CalcHexPasswordMd5("bar")
                });

                await dataConnection.Tab<LoginSessionDb>().InsertAsync(() => new LoginSessionDb
                {
                    Id = "right-login-sess-id",
                    CreateDt = DateTime.Now,
                    Expiry = DateTime.MaxValue,
                    LoginExpiry = DateTime.MaxValue,
                });

                await dataConnection.Tab<TokenSessionDb>().InsertAsync(() => new TokenSessionDb
                {
                    Id = "right-token-sess-id",
                    LoginId = "right-login-sess-id",
                    ClientId = "right-client",
                    AuthCode = "right-code",
                    RedirectUri = "http://right.ru/cb",
                    Scope = "openid",
                    CreateDt = DateTime.Now
                });

                await dataConnection.Tab<ClientDb>().InsertAsync(() => new ClientDb
                {
                    Id = "right-client2",
                    Name = "test-client",
                    Enabled = MySqlBool.True,
                    PasswordHash = TestTools.PasswordHashCalculator.CalcHexPasswordMd5("bar")
                });

                await dataConnection.Tab<LoginSessionDb>().InsertAsync(() => new LoginSessionDb
                {
                    Id = "right-login-sess-id-2",
                    CreateDt = DateTime.Now,
                    Expiry = DateTime.MaxValue,
                    LoginExpiry = DateTime.MaxValue,
                });

                await dataConnection.Tab<TokenSessionDb>().InsertAsync(() => new TokenSessionDb
                {
                    Id = "right-token-sess-id-2",
                    LoginId = "right-login-sess-id-2",
                    ClientId = "right-client2",
                    AuthCode = "right-code2",
                    RedirectUri = "http://right.ru/cb",
                    Scope = "openid",
                    CreateDt = DateTime.Now
                });
            }
        }
    }
}
