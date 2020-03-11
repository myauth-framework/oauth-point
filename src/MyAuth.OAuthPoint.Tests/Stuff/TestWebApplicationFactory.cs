using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Services;
using MyLab.WebErrors;
using MyLab.WebInteraction;

namespace MyAuth.OAuthPoint.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile("appsettings.Development.json", true, true);
                    config.AddJsonFile("appsettings.override.json", true, true);
                });
        }
    }
    
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) 
            : base(configuration, new Configurator())
        {
            
        }
        
        class Configurator : IAppConfigurator
        {
            public void Configure(IServiceCollection services, IConfiguration configuration)
            {
                services
                    .AddMvc(o => o.AddExceptionProcessing())
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                
                services.AddSingleton<ILoginRegistry, TestLoginRegistry>();
                services.AddSingleton<IRefreshTokenRegistry, TestRefreshTokenRegistry>();
                services.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
                services.AddLogging(l => l.AddConsole());
                
                services.Configure<TokenIssuingOptions>(options =>
                    {
                        options.Issuer = TestTokenIssuingOptions.Options.Issuer;
                        options.Secret = TestTokenIssuingOptions.Options.Secret;
                        options.AccessTokenLifeTimeMin = TestTokenIssuingOptions.Options.AccessTokenLifeTimeMin;
                        options.RefreshTokenLifeTimeDays = TestTokenIssuingOptions.Options.RefreshTokenLifeTimeDays;
                    });

            }
        }
    }
}