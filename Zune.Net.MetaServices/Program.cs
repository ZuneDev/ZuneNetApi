using MetaBrainz.MusicBrainz;
using Zune.DB;
using Zune.Net;
using Zune.Net.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddSwaggerDocument();
builder.Services.AddResponseCaching();
builder.Host.ConfigureZuneDB(true);
builder.Services.AddTransient<ZuneNetContext>();
builder.Services.AddSingleton(new Query("Zune", "4.8", "https://github.com/xerootg/ZuneNetApi"));
builder.Services.AddTransient(typeof(WMIS));

var app = builder.Build();

// app.UseHttpLogging();

app.MapControllers();
app.UseResponseCaching();

app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();
