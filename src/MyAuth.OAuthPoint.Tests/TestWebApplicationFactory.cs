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
            return WebHost.CreateDefaultBuilder().UseStartup<TestStartup>();
        }
    }
    
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) 
            : base(configuration, new Configurator())
        {
            
        }
        
        /*public void ConfigureServices(IServiceCollection services)
        {
            AppConfigurator.Configure(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.AddResponceSourceHeader();
            app.UseMvc();
        }*/
        
        class Configurator : IAppConfigurator
        {
            public void Configure(IServiceCollection services, IConfiguration configuration)
            {
                services
                    .AddMvc(o => o.AddExceptionProcessing())
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                
                services.AddSingleton<ILoginRegistry, TestLoginRegistry>();
                services.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
                services.AddLogging(l => l.AddConsole());

                LoadClients(services);
            }
            
            private void LoadClients(IServiceCollection services)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "clients.json");
                var registry = DefaultClientRegistry.LoadFromJson(File.ReadAllText(filePath));
                services.AddSingleton<IClientRegistry>(registry);
            }
        }
    }
}