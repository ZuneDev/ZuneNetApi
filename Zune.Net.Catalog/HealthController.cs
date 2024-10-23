using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Zune.Net.Helpers;
using CommunityToolkit.Diagnostics;
using System.Linq;
using Zune.DB;

namespace Zune.Net.Catalog;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly ZuneNetContext database;

    public HealthController(ZuneNetContext _database)
    {
        database = _database;
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
        }
        _checks.Add(("MusicBrainz", mbException));

        // Check DB
        Exception dbException = null;
        try
        {
            await database.GetSingleAsync()
                .WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            dbException = ex;
        }
        _checks.Add(("Database", dbException));

        var allHealthy = _checks.All(t => t.Item2 is null);
        var statuses = _checks.ToDictionary(t => t.Item1, t => t.Item2 is null ? "OK" : "Unhealthy");

        return allHealthy
            ? Ok(statuses)
            : StatusCode(503, statuses);
    }
}
