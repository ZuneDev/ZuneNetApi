using Atom.Xml;
using MetaBrainz.MusicBrainz;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/track/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class TrackController : Controller
    {
        private readonly Query _mb;

        public TrackController(Query query)
        {
            _mb = query;
        }

        [HttpGet, Route("")]
        public ActionResult<Feed<Track>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            string queryStr = queries[0];
            var results = _mb.FindAllRecordings(queryStr, simple: true);
            var updated = DateTime.Now;
            Feed<Track> feed = new()
            {
                Id = "tracks",
                Title = "tracks",
                Links = { new(Request.Path) },
                Updated = updated,
                Entries = new()
            };
            
            // Add results to feed
            foreach (var result in results)
            {
                if (feed.Entries.Count == 40) break;

                var mb_rec = result.Item;
                var mb_artist = mb_rec.ArtistCredit[0].Artist;

                MiniArtist artist = new()
                {
                    Id = mb_artist.Id,
                    Title = mb_artist.Name
                };

                Track track = new()
                {
                    Id = mb_rec.Id.ToString(),
                    Title = mb_rec.Title,
                    ArtistName = artist.Title,
                    PrimaryArtist = artist,
                    AlbumArtist = artist,
                    Duration = mb_rec.Length ?? TimeSpan.Zero,
                    Artists = new List<MiniArtist>(mb_rec.ArtistCredit.Count),
                    Updated = updated,
                };

                var mb_album = mb_rec.Releases?[0];
                if (mb_album != null)
                {
                    track.AlbumId = mb_album.Id;
                    track.AlbumTitle = mb_album.Title;
                }

                foreach (var mb_credit in mb_rec.ArtistCredit)
                {
                    track.Artists.Add(new()
                    {
                        Id = mb_credit.Artist.Id,
                        Title = mb_credit.Name
                    });
                }

                feed.Entries.Add(track);
            }

            return feed;
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult Details(Guid mbid)
        {
            return Ok();
        }
    }
}
