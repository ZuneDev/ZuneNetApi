using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/album/")] // Desktop app
    [Route("/v3.0/{culture}/music/album/")] // Zune HD
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class AlbumController : Controller
    {

        [HttpGet("")]
        public ActionResult<Feed<Album>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchAlbums(queries[0], Request.Path);
        }
        //"GET /v3.2/en-US/music/album/c7153846-046f-41b7-a9cd-a9d10751ae60 HTTP/1.1" 200 42271 "-" "-"
        [HttpGet("{mbid}"), HttpGet("details/{mbid}")]
        public ActionResult<Album> Details(Guid mbid)
        {
            return MusicBrainz.GetAlbumByMBID(mbid);
        }
    }
}
