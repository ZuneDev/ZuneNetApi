using Flurl;
using Flurl.Http;
using MetaBrainz.MusicBrainz;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Controllers
{
    [Route("/stream")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class StreamController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public StreamController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("music/{mbid}")]
        public async Task<ActionResult> DefaultStreaming(Guid mbid)
        {
            var track = MusicBrainz.GetTrackByMBID(mbid);
            var mb_rel = MusicBrainz._query.LookupRelease(track.Album.Id, Include.UrlRelationships);
            var mb_relgrps = MusicBrainz._query.BrowseReleaseGroups(mb_rel, 1, inc: Include.UrlRelationships);

            if (mb_relgrps.TotalResults == 0)
                return NotFound();

            var mb_relgrp = mb_relgrps.Results[0];
            var uri = mb_relgrp.Relationships.FirstOrDefault(r => r.Type == "allmusic")?.Url?.Resource;
            if (uri == null)
                return NotFound();

            string alid = uri.Segments[^1].ToUpperInvariant();
            var samples = await $"https://www.allmusic.com/album/{alid}/samples.json".GetJsonAsync<List<Newtonsoft.Json.Linq.JToken>>();
            string sampleUrl = samples[track.TrackNumber - 1]["sample"].ToString();

            return Redirect(sampleUrl);
        }
    }
}
