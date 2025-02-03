using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

            app.UseCommonRouting();

            app.Run();
        }
    }
}