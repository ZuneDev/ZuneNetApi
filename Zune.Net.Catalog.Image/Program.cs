using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Zune.Net.Catalog.Image
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Host.ConfigureZuneDB();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();

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