using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint;
using MyAuth.OAuthPoint.Client;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using Xunit.Abstractions;

namespace FuncTests
{
    public partial class MyAuthPointBehavior
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<MyAuthOAuthPointDbInitializer> _dbFixture;
        private readonly TestApi<Startup, IOidcServiceContractV1> _oidcTestApi;

        public MyAuthPointBehavior(ITestOutputHelper output, TmpDbFixture<MyAuthOAuthPointDbInitializer> dbFixture)
        {
            _output = output;
            _dbFixture = dbFixture;

            dbFixture.Output = output;

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

        public void Dispose()
        {
            _oidcTestApi?.Dispose();
        }
    }
}