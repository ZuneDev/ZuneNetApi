using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Catalog.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/track/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class TrackController : Controller
    {

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
    }
}
