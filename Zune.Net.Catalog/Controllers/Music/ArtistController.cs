using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Catalog.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController : Controller
    {
        private readonly MusicBrainz _mb;

        public ArtistController(MusicBrainz mb)
        {
            _mb = mb;
        }

        [HttpGet, Route("")]
        public ActionResult<Feed<Artist>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return _mb.SearchArtists(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult<Artist> Details(Guid mbid)
        {
            return _mb.GetArtistByMBID(mbid);
        }

        [HttpGet, Route("{mbid}/tracks")]
        public ActionResult<Feed<Track>> Tracks(Guid mbid)
        {
            if (!Request.Query.TryGetValue("chunkSize", out var chunkSizeStrs) || chunkSizeStrs.Count != 1)
                return BadRequest();

            return _mb.GetArtistTracksByMBID(mbid, Request.Path, int.Parse(chunkSizeStrs[0]));
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid)
        {
            return _mb.GetArtistAlbumsByMBID(mbid, Request.Path);
        }
    }
}
