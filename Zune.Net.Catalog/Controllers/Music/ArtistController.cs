using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Features;
using Zune.Net.Helpers;
using Zune.Net.Ontology;
using Zune.Xml.Catalog;
using ListenBrainz = Zune.Net.Helpers.ListenBrainz;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController(ZuneNetContext database, BatchIdMapper batchIdMapper) : Controller
    {
        [HttpGet, Route("")]
        public ActionResult<Feed<Artist>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchArtists(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid}")]
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
                    var artistImageEntry = await database.AddImageAsync(artistImageUrl);

                    artist.BackgroundImage = new()
                    {
                        Id = artistImageEntry.Id
                    };
                }
            }

            return artist;
        }

        [HttpGet, Route("{mbid}/tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks(Guid mbid,
            [FromQuery] int? chunkSize = null, [FromQuery(Name = "orderby")] string orderBy = null)
        {
            Feed<Track> feed = new()
            {
                Id = mbid.ToString(),
                Title = "albums",
                Links = { new Link(Request.Path) },
                Entries = await MusicBrainz.GetArtistTracksByMBID(mbid, chunkSize)
                    .Take(chunkSize ?? 20)
                    .ToListAsync(),
                Updated = DateTime.Now,
            };

            orderBy = orderBy?.ToLowerInvariant();
            if (orderBy is "mostplayed" or "playrank")
            {
                // We need data from ListenBrainz to sort this
                var trackMbids = feed.Entries.Select(track => track.Id).ToList();
                var popularities = await ListenBrainz.GetRecordingPopularity(trackMbids);

                foreach (var track in feed.Entries)
                    if (popularities.TryGetValue(track.Id, out var popularity))
                        track.Popularity = popularity;
            }
            
            var cultureFeature = HttpContext.Features.Get<ICultureFeature>();
            var culture = cultureFeature?.GetCultureInfo() ?? CultureInfo.CurrentCulture;

            Comparison<Track> sortComparer = orderBy switch
            {
                "title" => (a, b) => culture.CompareInfo.Compare(a.SortTitle ?? a.Title.Value, b.SortTitle ?? b.Title.Value),
                "mostplayed" or
                "playrank" or
                _ => (a, b) => b.Popularity.CompareTo(a.Popularity),
            };

            feed.Entries.Sort(sortComparer);
            return feed;
        }

        [HttpGet, Route("{mbid}/albums")]
        [HttpGet, Route("{mbid}/relatedAlbums")]
        public async Task<ActionResult<Feed<Album>>> Albums(Guid mbid,
            [FromQuery] int? chunkSize = null, [FromQuery(Name = "orderby")] string orderBy = null)
        {
            Feed<Album> feed = new()
            {
                Id = mbid.ToString(),
                Title = "albums",
                Links = { new Link(Request.Path) },
                Entries = await MusicBrainz.GetArtistAlbumsByMBID(mbid, chunkSize)
                    .Take(chunkSize ?? 20)
                    .ToListAsync(),
                Updated = DateTime.Now,
            };
            
            orderBy = orderBy?.ToLowerInvariant();
            if (orderBy is "mostplayed" or "playrank")
            {
                // We need data from ListenBrainz to sort this
                var releaseMbids = feed.Entries.Select(album => album.Id).ToList();
                var popularities = await ListenBrainz.GetReleasePopularity(releaseMbids);

                foreach (var album in feed.Entries)
                    if (popularities.TryGetValue(album.Id, out var popularity))
                        album.Popularity = popularity;
            }
            
            var cultureFeature = HttpContext.Features.Get<ICultureFeature>();
            var culture = cultureFeature?.GetCultureInfo() ?? CultureInfo.CurrentCulture;

            Comparison<Album> sortComparer = orderBy?.ToLowerInvariant() switch
            {
                "title" => (a, b) => culture.CompareInfo.Compare(a.SortTitle ?? a.Title.Value, b.SortTitle ?? b.Title.Value),
                "mostplayed" or
                "playrank" => (a, b) => b.Popularity.CompareTo(a.Popularity),
                "releasedate" or
                _ => (a, b) => b.ReleaseDate.Year.CompareTo(a.ReleaseDate.Year)
            };

            feed.Entries.Sort(sortComparer);
            return feed;
        }

 	    [HttpGet, Route("{mbid:guid}/deviceBackgroundImage")]
        [HttpGet, Route("{mbid:guid}/primaryImage")]
        public async Task<IActionResult> PrimaryImage(Guid mbid,
            [FromQuery] bool resize = false, [FromQuery(Name = "width")] int? requestedWidth = null,
            [FromQuery] string contentType = MediaTypeNames.Image.Jpeg)
        {
            var dcArtist = await Discogs.GetDCArtistByMBID(mbid, batchIdMapper);
            var imgUrl = dcArtist["images"]?
                .FirstOrDefault(i => i.Value<string>("type") == "primary")?
                .Value<string>("uri");

            return await this.GetAndResizeImageAsync(imgUrl, resize, requestedWidth, contentType);
        }

        [HttpGet, Route("{mbid}/biography")]
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

        [HttpGet, Route("{mbid}/images")]
        public async Task<ActionResult<Feed<Image>>> Images(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            var updated = DateTime.Now;
            var dcid = dc_artist.Value<ulong>("id");

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
                    // TODO: Let's not use a special format just for Discogs images
                    // Encode DCID and image index in ID
                    Span<byte> bytes = stackalloc byte[16];
                    BitConverter.TryWriteBytes(bytes, (uint)0);
                    BitConverter.TryWriteBytes(bytes[sizeof(uint)..], (ushort)idx);
                    BitConverter.TryWriteBytes(bytes[(sizeof(uint) + sizeof(ushort))..], dcid);
                    var imgId = new Guid(bytes);

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
                }).ToList();
            }

            return feed;
        }

        [HttpGet, Route("{mbid}/similarArtists")]
        public async Task<ActionResult<Feed<Artist>>> SimilarArtists(Guid mbid)
        {
            return await LastFM.GetSimilarArtistsByMBID(mbid);
        }
    }
}
