using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                    
                    #if DEBUG
                    config.AddJsonFile("appsettings.Development.json", true, true);
                    #endif
                    
                    config.AddJsonFile("appsettings.override.json", true, true);
                })
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory());
    }
}