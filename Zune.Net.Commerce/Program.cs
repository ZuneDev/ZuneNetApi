using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zune.Net;
using Zune.Net.Middleware;

namespace CommerceZuneNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureLogging(cfg => 
            {
                cfg.AddConsole();
            });

            // Add services to the container.
            builder.Services.AddControllers().AddXmlSerializerFormatters(); 

            builder.Host.ConfigureZuneDB();

            var app = builder.Build();

            app.UseHttpLogging();

            // Configure the HTTP request pipeline.
            app.UseRequestBuffering();

            app.UseRouting();

            app.UseWlidAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", ctx =>
                {
                    return Task.FromResult(new OkObjectResult("Welcome to the Social"));
                });
            });

            app.Run();
        }
    }
}
