using Zune.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllersWithViews(o => o.UseZestFormatters());

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
