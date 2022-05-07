var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/{locale}/ZunePCClient/{version}/{file}.xml", (string locale, string version, string file) =>
{
    string filePath = Path.Combine(app.Environment.ContentRootPath, "Resources", file + ".xml");
    string zuneConfig = File.ReadAllText(filePath);

    return zuneConfig;
});

app.Run();
