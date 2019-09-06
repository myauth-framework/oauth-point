using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAuth.OAuthPoint.Services;
using MyLab.WebErrors;

namespace MyAuth.OAuthPoint.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<ILoginRegistry, TestLoginRegistry>();
                services.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
                services.AddLogging(l => l.AddConsole());

                LoadClients(services);
            });
        }

        private void LoadClients(IServiceCollection services)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "clients.json");
            var registry = DefaultClientRegistry.LoadFromJson(File.ReadAllText(filePath));
            services.AddSingleton<IClientRegistry>(registry);
        }
    }
}