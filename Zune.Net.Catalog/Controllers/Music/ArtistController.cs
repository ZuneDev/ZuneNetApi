using Atom.Xml;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/artist/")]
    [Route("/v3.0/{culture}/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController : Controller
    {
        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;

        public ArtistController(ILogger<ArtistController> logger, ZuneNetContext database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpGet("")]
        public ActionResult<Feed<Artist>> Search(string q)
        {
            if(string.IsNullOrEmpty(q))
            {
                return BadRequest();
            }
            return MusicBrainz.SearchArtists(q, Request.Path);
        }

        [HttpGet("{mbid}")]
        public async Task<ActionResult<Artist>> Details(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            Artist artist = MusicBrainz.MBArtistToArtist(mb_artist);
            artist.Images = new() { new() { Id = mbid } };

            if (dc_artist != null)
            {
                artist.Biography = dc_artist.Value<string>("profile");
                artist.Links.Add(new(Request.Path.Value + "biography", relation: "zune://artist/biography"));

                var dc_artist_image = dc_artist.Value<JArray>("images")?.FirstOrDefault(i => i.Value<string>("type") == "primary");
                if (dc_artist_image != null)
                {
                    string artistImageUrl = dc_artist_image.Value<string>("uri");
                    var artistImageEntry = await _database.AddImageAsync(artistImageUrl);

                    artist.BackgroundImage = new()
                    {
                        Id = artistImageEntry.Id
                    };
                }
            }

            return artist;
        }

        [HttpGet("{mbid}/tracks")]
        public ActionResult<Feed<Track>> Tracks(Guid mbid, string orderby = "unset", int chunkSize = 10)
        {
            var tracks = MusicBrainz.GetArtistTracksByMBID(mbid, Request.Path, 100);
            var toSort = tracks.Entries;
            try
            {
                switch (orderby.ToLowerInvariant())
                {
                    case "playrank":
                        tracks.Entries.Sort((a, b) => a.Popularity.CompareTo(b));
                        break;
                }
            }
            catch { }
            tracks.Entries = tracks.Entries.Take(chunkSize).ToList();
            return tracks;
        }

        [HttpGet("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid, int chunkSize = 10, string orderby = "ReleaseDate")
        {
            var feed = MusicBrainz.GetArtistAlbumsByMBID(mbid, Request.Path);
            Comparison<Album> sortComparer = orderby.ToLowerInvariant() switch
            {
                "title" => (a, b) => (a.SortTitle ?? a.Title.Value).CompareTo(b.SortTitle ?? b.Title.Value),

                "mostplayed" or "playrank" => (a, b) => a.Popularity.CompareTo(b.Popularity),

                //default
                _ or "releasedate" => (a, b) => a.ReleaseDate.Year.CompareTo(b.ReleaseDate.Year),
            };
            feed.Entries.Sort(sortComparer);
            feed.Entries = feed.Entries.Take(chunkSize).ToList();
            return feed;
        }

        [HttpGet("{mbid}/biography")]
        public async Task<ActionResult<Entry>> Biography(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            if (dc_artist == null)
                return StatusCode(404);
            DateTime updated = DateTime.Now;

            return new Entry
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/biography",
                Title = mb_artist.Name,
                Links = { new(Request.Path) },
                Content = Discogs.DCProfileToBiographyContent(dc_artist.Value<string>("profile")),
                Updated = updated,
            };
        }
        
        [HttpGet("{mbid}/primaryImage")]
        public async Task<ActionResult> PrimaryImage(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            if (dc_artist == null)
            {
                _logger.LogDebug($"No artist found for mbid: {mbid}");
                return StatusCode(404);
            }

            string imgUrl = dc_artist["images"].First(i => i.Value<string>("type") == "primary").Value<string>("uri");
            var imgResponse = await imgUrl.GetAsync();
            if (imgResponse.StatusCode != 200)
                return StatusCode(imgResponse.StatusCode);
            return File(await imgResponse.GetStreamAsync(), "image/jpeg");
        }

        [HttpGet("{mbid}/images")]
        public async Task<ActionResult<Feed<Image>>> Images(Guid mbid, int chunkSize = 40)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            DateTime updated = DateTime.Now;
            if(dc_artist == null)
            {
                return StatusCode(404);
            }
            int dcid = dc_artist.Value<int>("id");
            byte[] zero = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            Feed<Image> feed = new()
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/images",
                Title = mb_artist.Name,
                Links = { new(Request.Path) },
                Updated = updated,
            };

            var images = dc_artist.Value<JToken>("images");
            if (images != null)
            {
                feed.Entries = images.Select((j, idx) =>
                {
                    // Encode DCID and image index in ID
                    Guid imgId = new(dcid, (short)idx, 0, zero);

                    return new Image
                    {
                        Id = imgId,
                        Instances = new()
                        {
                            new()
                            {
                                Id = imgId,
                                Url = j.Value<string>("uri"),
                                Format = "jpg",
                                Width = j.Value<int>("width"),
                                Height = j.Value<int>("height"),
                            }
                        }
                    };
                }).Take(chunkSize).ToList();
            }

            return feed;
        }

        [HttpGet("{mbid}/similarArtists")]
        [HttpGet("{mbid}/influencers")]
        public async Task<ActionResult<Feed<Artist>>> SimilarArtists(Guid mbid, int chunkSize = 10)
        {
            var similar = await LastFM.GetSimilarArtistsByMBID(mbid);
            similar.Entries = similar.Entries.Take(chunkSize).ToList();
            return similar;
        }
    }
}
