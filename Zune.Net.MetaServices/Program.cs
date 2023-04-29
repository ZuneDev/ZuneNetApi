using Zune.Net;
using Zune.Net.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddSingleton(typeof(WMIS));
builder.Host.ConfigureZuneDB();

var app = builder.Build();

app.MapControllers();

app.Run();
