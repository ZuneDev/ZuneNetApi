using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zune.Net.Middleware;

namespace Zune.Net.Login;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Host.ConfigureZuneDB();

        builder.Services.AddControllers();

        var app = builder.Build();
        // app.UseHttpLogging();

        // Configure the HTTP request pipeline.

        app.UseWlidAuthorization();

        app.MapControllers();

        app.Run();
    }
}