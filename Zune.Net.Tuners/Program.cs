using Zune.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/{locale}/ZunePCClient/{version}/{file}.xml", (string locale, string version, string file) =>
{
    // TODO: Don't load from file paths. It's unsafe, and the files aren't copied into the Docker container.
    string filePath = Path.Combine(app.Environment.ContentRootPath, "Resources", file + ".xml");
    string zuneConfig = File.ReadAllText(filePath);

    return zuneConfig;
});
app.MapHomeRoute();

app.Run();
