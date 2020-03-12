using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using MyLab.RemoteConfig;

namespace MyAuth.OAuthPoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .LoadRemoteConfigConnectionFromEnvironmentVars()
                .AddRemoteConfiguration()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory());
    }
}