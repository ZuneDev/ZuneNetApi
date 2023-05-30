var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// app.UseHttpLogging();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
