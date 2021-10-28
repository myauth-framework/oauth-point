using LinqToDB;
using LinqToDB.DataProvider.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyAuth.OAuthPoint.Services;
using MyLab.Db;
using Newtonsoft.Json;

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
            services.AddControllers().AddNewtonsoftJson(o => o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
            services
                .AddSingleton<IX509CertificateProvider, X509CertificateProvider>()
                .AddSingleton<PasswordHashCalculator>()
                .AddSingleton<ITokenService, TokenService>()
                .AddSingleton<ISessionCreator, SessionCreator>()
                .AddSingleton<ISessionProvider, SessionProvider>()
                .AddSingleton<ISessionCompleter, SessionCompleter>()
                .AddDbTools(Configuration, new MySqlDataProvider(ProviderName.MySql))
                .AddLocalization(lo => lo.ResourcesPath = "Resources");

            services.Configure<AuthTimingsOptions>(Configuration.GetSection("Auth:Timing"));
            services.Configure<AuthStoringOptions>(Configuration.GetSection("Auth:Storing"));
            services.Configure<AuthEndpointsOptions>(Configuration.GetSection("Auth:Endpoints"));
            services.Configure<TokenIssuingOptions>(Configuration.GetSection("Auth:Issuing"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
