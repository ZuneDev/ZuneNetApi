﻿using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/genre/")]
    [Route("/v3.0/{culture}/music/genre/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class GenreController : Controller
    {

        [HttpGet("")]
        public ActionResult<Feed<Genre>> Genres()
        {
            return MusicBrainz.GetGenres(Request.Path);
        }

        [HttpGet("{id}")]
        public ActionResult<Genre> Details(string id)
        {
            if (Guid.TryParse(id, out Guid mbid))
                return MusicBrainz.GetGenreByMBID(mbid);
            return MusicBrainz.GetGenreByZID(id);
        }

        [HttpGet("{id}/albums")]
        public ActionResult<Feed<Album>> Albums(string id)
        {
            if (Guid.TryParse(id, out Guid mbid))
                return MusicBrainz.GetGenreAlbumsByMBID(mbid, Request.Path);
            return MusicBrainz.GetGenreAlbumsByZID(id, Request.Path);
        }

        // Not actually used by Zune, but hey, might as well
        [HttpGet("{id}/tracks")]
        public ActionResult<Feed<Track>> Tracks(string id)
        {
            if (Guid.TryParse(id, out Guid mbid))
                return MusicBrainz.GetGenreTracksByMBID(mbid, Request.Path);
            return MusicBrainz.GetGenreTracksByZID(id, Request.Path);
        }

        [HttpGet("{id}/artists")]
        public ActionResult<Feed<Artist>> Artists(string id)
        {
            if (Guid.TryParse(id, out Guid mbid))
                return MusicBrainz.GetGenreArtistsByMBID(mbid, Request.Path);
            return MusicBrainz.GetGenreArtistsByZID(id, Request.Path);
        }
    }
}
