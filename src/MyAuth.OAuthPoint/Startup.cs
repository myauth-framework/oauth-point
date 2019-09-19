using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotRedis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.OAuthPoint.Services;
using MyLab.RedisManager;
using MyLab.WebErrors;
using MyLab.WebInteraction;

namespace MyAuth.OAuthPoint
{
    public class Startup
    {
        protected IAppConfigurator AppConfigurator { get; }

        public Startup(IConfiguration configuration)
            :this(configuration, new Configurator())
        {
            Configuration = configuration;
        }
        
        internal Startup(IConfiguration configuration, IAppConfigurator appConfigurator)
        {
            AppConfigurator = appConfigurator;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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
        }
        
        class Configurator : IAppConfigurator
        {
            public void Configure(IServiceCollection services, IConfiguration configuration)
            {
                services
                    .AddMvc(o => o.AddExceptionProcessing())
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
                services.AddRedisManager(configuration);
                services.AddSingleton<ILoginRegistry, DefaultLoginRegistry>();
                services.AddSingleton<IRefreshTokenRegistry, DefaultRefreshTokenRegistry>();
                services.AddSingleton(configuration);

                services.Configure<ExceptionProcessingOptions>(o =>
#if DEBUG
                        o.HideError = false
#else
                    o.HideError = true
#endif  
                );
                services.Configure<TokenIssuingOptions>(configuration.GetSection("TokenIssuing"));

                services.AddLogging(builder => builder.AddConsole());

#if DEBUG
                Redis.Debugger = new RedisDebugger((request, response) =>
                {
                    Console.WriteLine("REDIS DEBUG >>> " + request);
                    Console.WriteLine("\tRequest: \t" + request);
                    Console.WriteLine("\tResponse: \t" + response);
                });
#endif

                LoadClients(services);
            }
            
            private void LoadClients(IServiceCollection services)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "clients.json");

                if (File.Exists(filePath))
                {
                    var registry = DefaultClientRegistry.LoadFromJson(File.ReadAllText(filePath));
                    services.AddSingleton<IClientRegistry>(registry);
                }
            }
        }
    }
}