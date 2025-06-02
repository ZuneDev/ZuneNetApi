using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Zune.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/{locale}/ZunePCClient/{version}/{fileName}", (string locale, string version, string fileName) =>
{
    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = assembly.GetManifestResourceNames()
        .SingleOrDefault(str => str.EndsWith($".{fileName}"));
    if (resourceName is null)
        return Results.NotFound();

    byte[] content;

    using (var stream = assembly.GetManifestResourceStream(resourceName))
    {
        if (stream is null)
            return Results.NotFound();
        
        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);
        content = memoryStream.ToArray();
    }
    
    new FileExtensionContentTypeProvider().TryGetContentType(resourceName, out var contentType);
    return Results.File(content, contentType, fileName);
});
app.MapHomeRoute();

app.Run();
