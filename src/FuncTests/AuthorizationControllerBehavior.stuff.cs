using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyAuth.OAuthPoint.Db;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using MySqlX.XDevAPI.CRUD;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public partial class AuthorizationControllerBehavior : IDisposable, IClassFixture<TmpDbFixture<MyAuthOAuthPointDbInitializer>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IOidcContractV1> _oidcTestApi;

        private const string TestConfigLoginEndpoint = "http://host.net/login";
        private const string TestConfigDefaultErrorEndpoint = "http://host.net/error";

        public AuthorizationControllerBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
        {
            _output = output;
            _dbFixture = dbFixture;

            dbFixture.Output = output;

            _oidcTestApi = new TestApi<Startup, IOidcContractV1>()
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

        public void Dispose()
        {
            _oidcTestApi?.Dispose();
        }

        private DataDbInitializer CreateDataInitializerForCookieTest(
            string clientId, 
            string loginSessId,
            string tokenSessId,
            string redirectUri,
            string scopes,
            string authCode = null)
        {
            var dataInitializer = DataDbInitializer.Create(clientId, redirectUri, scopes);

            dataInitializer.ClientRedirectUris = new[]
            {
                new ClientAvailableUriDb
                {
                    ClientId = clientId,
                    Uri = redirectUri
                }
            };

            dataInitializer.LoginSessions = new[]
            {
                new LoginSessionDb
                {
                    Id = loginSessId,
                    Expiry = DateTime.MaxValue,
                    Status = LoginSessionDbStatus.Started
                }
            };

            dataInitializer.TokenSessions = new[]
            {
                new TokenSessionDb
                {
                    ClientId = clientId,
                    LoginId = loginSessId,
                    Id = tokenSessId,
                    Scope = scopes,
                    RedirectUri = redirectUri,
                    AuthCode= authCode,
                    Status = TokenSessionDbStatus.Started
                }
            };

            return dataInitializer;
        }
    }
}