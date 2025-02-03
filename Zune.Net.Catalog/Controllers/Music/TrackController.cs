using Atom.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/track/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class TrackController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public TrackController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet, Route("")]
        public ActionResult<Feed<Track>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchTracks(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult<Track> Details(Guid mbid)
        {
            return MusicBrainz.GetTrackByMBID(mbid);
        }

        [HttpGet, Route("{mbid}/similarTracks")]
        public async Task<ActionResult<Feed<Track>>> SimilarTracks(Guid mbid)
        {
            return await LastFM.GetSimilarTracksByMBID(mbid);
        }
    }
}
