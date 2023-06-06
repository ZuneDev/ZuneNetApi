using Zune.Net;
using Zune.Net.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(x=>x.AddConsole());

builder.Services.AddControllers();

builder.Services.AddControllersWithViews(o => o.UseZestFormatters());

var app = builder.Build();

// initialize so we have a cache to protect from overcalling
MusicBrainz.Initialize(app.Environment);

// Configure the HTTP request pipeline.

// app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
