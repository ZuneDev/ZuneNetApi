using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Zune.Net.Helpers;
using CommunityToolkit.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zune.DB;

namespace Zune.Net.Catalog;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly IServiceProvider _services;
    private readonly ILogger<HealthController> _logger;
    // private readonly ZuneNetContext database;

    public HealthController(IServiceProvider serviceProvider, ILogger<HealthController> logger)
    {
        _services = serviceProvider;
        _logger = logger;
        // database = _database;
    }

    [HttpGet]
    public async Task<IActionResult> HealthCheck()
    {
        List<(string, Exception)> _checks = new();

        // Check MusicBrainz
        Exception mbException = null;
        try
        {
            var artist = MusicBrainz.GetArtistByMBID(Guid.Parse("534ee493-bfac-4575-a44a-0ae41e2c3fe4"));
            Guard.IsNotNull(artist);
        }
        catch (Exception ex)
        {
            mbException = ex;
            _logger.LogError(ex, "Failed to use MusicBrainz API");
        }
        _checks.Add(("MusicBrainz", mbException));

        // Check DB
        Exception dbException = null;
        try
        {
            var database = _services.GetRequiredService<ZuneNetContext>();
            await database.GetSingleAsync()
                .WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (ZuneNetConfigurationException ex)
        {
            dbException = ex;
            
            var ctx = _services.GetRequiredService<HostBuilderContext>();
            var configJson = ctx.Configuration.SerializeToJson();
            
            var writer = new StringWriter();
            var jsonWriter = new JsonTextWriter(writer);
            await configJson.WriteToAsync(jsonWriter);
            
            _logger.LogError("Failed to configure ZuneDB");
            _logger.LogError("{Config}", writer.ToString());
        }
        catch (Exception ex)
        {
            dbException = ex;
            _logger.LogError(ex, "Failed to use ZuneDB");
            
            var ctx = _services.GetRequiredService<HostBuilderContext>();
            _logger.LogError("{ConnectionString}", ctx.Configuration
                .GetSection("ZuneNetContext")
                .Get<ZuneNetContextSettings>()
                .ConnectionString);
        }
        _checks.Add(("Database", dbException));

        var allHealthy = _checks.All(t => t.Item2 is null);
        var statuses = _checks.ToDictionary(t => t.Item1, t => t.Item2 is null ? "OK" : "Unhealthy");

        return allHealthy
            ? Ok(statuses)
            : StatusCode(503, statuses);
    }
}
