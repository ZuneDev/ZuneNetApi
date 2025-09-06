using Atom.Xml;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.DataProviders;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController(ZuneNetContext database, AggregatedMediaProvider mediaProvider) : Controller
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
            var id = new MediaId(mbid, KnownMediaSources.MusicBrainz, MediaType.Artist);
            var artist = await mediaProvider.GetArtist(id);

            var biography = await mediaProvider.GetArtistBiography(id);
            artist.Biography = biography?.Value;

            var artistImageUrl = await mediaProvider.GetArtistPrimaryImage(id);
            if (artistImageUrl is not null)
            {
                var artistImageEntry = await database.AddImageAsync(artistImageUrl);
                artist.BackgroundImage = new()
                {
                    Id = artistImageEntry.Id
                };
            }

            return artist;
        }

        [HttpGet, Route("{mbid}/tracks")]
        public ActionResult<Feed<Track>> Tracks(Guid mbid)
        {
            if (!Request.Query.TryGetValue("chunkSize", out var chunkSizeStrs) || chunkSizeStrs.Count != 1)
                return BadRequest();

            return MusicBrainz.GetArtistTracksByMBID(mbid, Request.Path, int.Parse(chunkSizeStrs[0]));
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid)
        {
            var feed = MusicBrainz.GetArtistAlbumsByMBID(mbid, Request.Path);

            Comparison<Album> sortComparer = (a, b) => a.ReleaseDate.Year.CompareTo(b.ReleaseDate.Year);
            if (Request.Query.TryGetValue("orderby", out var orderByValue))
            {
                string orderBy = orderByValue.Single().ToLower();
                switch (orderBy)
                {
                    case "title":
                        sortComparer = (a, b) => (a.SortTitle ?? a.Title.Value).CompareTo(b.SortTitle ?? b.Title.Value);
                        break;

                    case "mostplayed":
                        sortComparer = (a, b) => a.Popularity.CompareTo(b.Popularity);
                        break;
                }
            }

            feed.Entries.Sort(sortComparer);
            return feed;
        }

        [HttpGet, Route("{mbid}/primaryImage")]
        public async Task<ActionResult> PrimaryImage(Guid mbid)
        {
            var id = new MediaId(mbid, KnownMediaSources.MusicBrainz, MediaType.Artist);
            var imgUrl = await mediaProvider.GetArtistPrimaryImage(id);

            if (imgUrl is null)
                return NotFound();

            var imgResponse = await imgUrl.GetAsync();
            if (imgResponse.StatusCode is not 200)
                return StatusCode(imgResponse.StatusCode);

            return File(await imgResponse.GetStreamAsync(), "image/jpeg");
        }

        [HttpGet, Route("{mbid}/biography")]
        public async Task<ActionResult<Entry>> Biography(Guid mbid)
        {
            var mediaId = new MediaId(mbid, KnownMediaSources.MusicBrainz);
            var bioContent = await mediaProvider.GetArtistBiography(mediaId);
            DateTime updated = DateTime.Now;

            if (bioContent is null)
                return StatusCode(404);

            var mbArtist = MusicBrainz.GetArtistByMBID(mbid);

            return new Entry
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/biography",
                Title = mbArtist.Title,
                Links = { new(Request.Path) },
                Content = bioContent,
                Updated = updated,
            };
        }

        [HttpGet, Route("{mbid}/images")]
        public async Task<ActionResult<Feed<Image>>> Images(Guid mbid)
        {
            Feed<Image> feed = new()
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/images",
                Links = { new(Request.Path) },
                Updated = DateTime.Now,
                Entries = [],
            };

            var id = new MediaId(mbid, KnownMediaSources.MusicBrainz, MediaType.Artist);
            var artist = await mediaProvider.GetArtist(id);
            feed.Title = artist.Title;

            var images = mediaProvider.GetArtistImages(id);
            await foreach (var imageUrl in images)
            {
                // Register image URL
                var imageDbEntry = await database.AddImageAsync(imageUrl);

                var image = new Image
                {
                    Id = imageDbEntry.Id,
                    Instances =
                    [
                        new()
                        {
                            Id = imageDbEntry.Id,
                            Url = imageUrl,
                            Format = "jpg",
                        }
                    ]
                };

                feed.Entries.Add(image);
            }

            return feed;
        }

        [HttpGet, Route("{mbid}/similarArtists")]
        public async Task<ActionResult<Feed<Artist>>> SimilarArtists(Guid mbid)
        {
            return await DataProviders.LastFM.LastFM.GetSimilarArtistsByMBID(mbid);
        }
    }
}
