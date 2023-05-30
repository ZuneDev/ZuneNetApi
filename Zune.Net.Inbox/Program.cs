using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zune.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Host.ConfigureZuneDB(true);

var app = builder.Build();

// app.UseHttpLogging();

app.MapControllers();

app.Run();
