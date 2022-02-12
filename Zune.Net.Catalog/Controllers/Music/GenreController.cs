using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Catalog.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/genre/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class GenreController : Controller
    {
        private readonly MusicBrainz _mb;

        public GenreController(MusicBrainz mb)
        {
            _mb = mb;
        }

        [HttpGet, Route("")]
        public ActionResult<Feed<Genre>> Genres()
        {
            return _mb.GetGenres(Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult<Genre> Details(Guid mbid)
        {
            return _mb.GetGenreByMBID(mbid);
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(string zid)
        {
            return _mb.GetGenreAlbumsByZID(zid, Request.Path);
        }
    }
}
