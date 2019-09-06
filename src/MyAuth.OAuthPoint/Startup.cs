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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(o => o.AddExceptionProcessing())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddRedisManager(Configuration);
            services.AddSingleton<ILoginRegistry, DefaultLoginRegistry>();

            services.Configure<ExceptionProcessingOptions>(o =>
#if DEBUG
                    o.HideError = false
#else
                    o.HideError = true
#endif  
            );

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
    }
}