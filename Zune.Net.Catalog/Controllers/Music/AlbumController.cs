using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v{version:decimal}/{culture}/music/album/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class AlbumController : Controller
    {

        [HttpGet, Route("")]
        public ActionResult<Feed<Album>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchAlbums(queries[0], Request.Path);
        }

        [HttpGet("{mbid}"), HttpGet("details/{mbid}")]
        public ActionResult<Album> Details(Guid mbid)
        {
            return MusicBrainz.GetAlbumByMBID(mbid);
        }
    }
}
