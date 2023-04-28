var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

app.MapGet("/{locale}/ZunePCClient/{version}/{file}.xml", (string locale, string version, string file) =>
{
    string filePath = Path.Combine(app.Environment.ContentRootPath, "Resources", file + ".xml");
    Console.WriteLine($"Serving up {filePath}");
    string zuneConfig = File.ReadAllText(filePath);

    return zuneConfig;
});

app.Run();
