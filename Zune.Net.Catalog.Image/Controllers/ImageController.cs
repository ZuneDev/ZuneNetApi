using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Image.Controllers
{
    [Route("/test/{culture}/")]
    [Route("/v3.2/{culture}/")]
    [Route("/v3.0/{culture}/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ImageController : Controller
    {
        private static readonly ConcurrentDictionary<int, JObject> dcArtistCache = new();

        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;
        public ImageController(ZuneNetContext database, ILogger<ImageController> logger)
        {
            _database = database;
            _logger = logger;
        }

        [HttpGet("image/{id}")]
        public async Task<IActionResult> Image(Guid id, bool resize = true, int width = 480, string contenttype = "image/jpeg")
        {
            string? imageUrl = null;

            (var idA, var idB, var idC) = id.GetGuidParts();

            if (idC == 0)
            {
                // do better. this is... sketch.
                int dcid = unchecked((int)idA);

                // Get or update cached artist
                if (!dcArtistCache.TryGetValue(dcid, out var dc_artist))
                {
                    _logger.LogDebug("artist not in cache, adding");
                    dc_artist = await Discogs.GetDCArtistByDCID(dcid);
                    dcArtistCache.AddOrUpdate(dcid, _ => dc_artist, (_, _) => dc_artist);
                }

                // Get URL for requested image
                _logger.LogDebug("getting discogs artist images");
                var images = dc_artist.Value<JArray>("images");
                if (images != null && images.Count > idB)
                {
                    _logger.LogDebug("got discogs artist image, sending");
                    var thisImage = images[idB];
                    imageUrl = thisImage.Value<string>("uri");
                }
            } else
            {
                try
                {
                    var imageEntry = await _database.GetImageEntryAsync(id);
                    if (imageEntry != null)
                    {
                        if (!string.IsNullOrEmpty(imageEntry.Url))
                        {
                            _logger.LogDebug("using database backed image");
                            imageUrl = imageEntry.Url;
                        }
                    }
                } catch {}
            }

            if(string.IsNullOrEmpty(imageUrl))
            {
                return await ArtistImage(id, "failoverFromPrimaryEndpoint", resize, width, contenttype);
            }

            return await ReturnResizedImageAsync(imageUrl, resize, width, contenttype);
        }

        // i.e. http://image.catalog.zune.net/v3.0/en-US/music/track/f32bb0ab-59d6-4620-b239-e86dc68647a4/albumImage?width=240&height=240&resize=true

        [HttpGet("music/{imageKind}/{id}/{type}")]
        public async Task<IActionResult> ArtistImage(Guid id, string type, bool resize = true, int width = 480, string contenttype = "image/jpeg")
        {
            _logger.LogDebug($"Fetching image type: '{type}', starting with DC");
            // known types - deviceBackgroundImage, primaryImage, albumImage.
            // primaryImage is mostly what the Zune app asks for
            // deviceBackgroundImage is what is displayed  on the 'now playing' screen
            // albumImage is just the album cover.

            string imageUrl;
            try
            {
                imageUrl = await GetImageUrlFromDCAsync(id);
            }
            catch
            {
                _logger.LogDebug("DC failed for some reason, so we are now going to try Cover Archive");
                imageUrl = GetImageUrlFromCoverArchive(id);
            }

            return await ReturnResizedImageAsync(imageUrl, resize, width, contenttype);
        }

        private async Task<IActionResult> ReturnResizedImageAsync(string imageUrl, bool resize, int width, string contenttype)
        {
            var imgResponse = await imageUrl.GetAsync();

            if (imgResponse.StatusCode != 200)
            {
                NotFound();
            }

            var image = await SixLabors.ImageSharp.Image.LoadAsync(await imgResponse.GetStreamAsync());
            if (resize && image.Size.Width > width)
            {
                _logger.LogDebug("resizing");
                image.Mutate(x => x.Resize(width, 0));
            }

            using var stream = new MemoryStream();

            if (contenttype.Contains("jpeg"))
            {
                _logger.LogDebug("sending as jpg");
                image.Save(stream, new JpegEncoder());
            }
            else if (contenttype.Contains("bmp"))
            {
                _logger.LogDebug("bmp");
                image.Save(stream, new BmpEncoder());
            }
            else if (contenttype.Contains("png"))
            {
                _logger.LogDebug("sending as png");
                image.Save(stream, new PngEncoder());
            }

            return File(stream.ToArray(), contenttype);
        }

        private static async Task<string> GetImageUrlFromDCAsync(Guid id)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(id);

            return dc_artist.Value<JArray>("images")?
                .FirstOrDefault(i => i.Value<string>("type") == "primary")?
                .Value<string>("uri") ?? throw new FileNotFoundException();
        }

        private static string GetImageUrlFromCoverArchive(Guid id)
        {
            string? albumId = null;
            try
            {
                albumId = MusicBrainz.GetAlbumByRecordingId(id).Id;
            } catch
            {
            }
            if(string.IsNullOrEmpty(albumId))
            {
                albumId = id.ToString();
            }
            return $"https://coverartarchive.org/release/{albumId}/front";
        }
    }
}
