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

        [HttpGet, Route("")]
        public ActionResult<Feed<Genre>> Genres()
        {
            return MusicBrainz.GetGenres(Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult<Genre> Details(Guid mbid)
        {
            return MusicBrainz.GetGenreByMBID(mbid);
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid)
        {
            return MusicBrainz.GetGenreAlbumsByMBID(mbid, Request.Path);
        }
    }
}
