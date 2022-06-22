using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceZuneNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Config cfg = new(args);

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (cfg.Host != null)
                        webBuilder.UseUrls($"http://{cfg.Host}:{cfg.Port}", $"https://{cfg.Host}:{cfg.SslPort}");
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, s) =>
                {
                    s.AddSingleton(sp => cfg);
                    s.Configure<Zune.DB.ZuneNetContextSettings>(ctx.Configuration.GetSection("ZuneNetContext"));
                    s.AddSingleton<Zune.DB.ZuneNetContext>();
                });
        }
    }
}
