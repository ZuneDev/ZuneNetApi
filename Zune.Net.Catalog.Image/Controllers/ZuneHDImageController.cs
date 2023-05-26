using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Zune.DB;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Image
{
    [Route("/v3.0/{culture}/")]
    public class ZuneHDArtistImageController : Controller
    {
        private static readonly Uri defaultImage = new Uri("https://hellofromseattle.com/wp-content/uploads/sites/6/2020/03/Zune-Basic.png");

        private ILogger _logger;
        private static readonly ConcurrentDictionary<int, JObject> dcArtistCache = new();

        private static readonly int[] caaSupportedSizes = new[] { 250, 500, 1200 };


        private ZuneNetContext _database;
        public ZuneHDArtistImageController(ZuneNetContext database, ILogger<ZuneHDArtistImageController> logger)
        {
            _logger = logger;
            _database = database;
        }

        [HttpGet, Route("image/{id}")]
        public async Task<IActionResult> Image(Guid id, bool resize = true, int width = 480, string contenttype = "image/jpeg")
        {
            string? imageUrl = null;

            (var idA, var idB, var idC) = id.GetGuidParts();
            var imageEntry = await _database.GetImageEntryAsync(id);

            if (imageEntry != null)
            {
                imageUrl = imageEntry.Url;
                _logger.LogDebug("using database backed image");
            }
            else if (idC == 0)
            {
                // wtf is this???
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
            }
            if (string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    _logger.LogDebug("Trying discogs artistID");
                    (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(id);
                    if (dc_artist != null)
                        imageUrl = dc_artist["images"].First(i => i.Value<string>("type") == "primary").Value<string>("uri");
                }
                catch
                {
                    _logger.LogDebug("using coverart archive");
                    imageUrl = $"https://coverartarchive.org/release/{id}/front";
                }
            }

            var imgResponse = await imageUrl.GetAsync();
            if (imgResponse.StatusCode != 200)
            {
                _logger.LogDebug("failed to fetch image");
                return StatusCode(imgResponse.StatusCode);
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

        //width=480&resize=true&contenttype=image/jpeg
        [HttpGet("music/artist/{mbid}/deviceBackgroundImage")]
        public async Task<ActionResult> PrimaryImage(Guid mbid, string contenttype = "image/jpeg", int width = 480, bool resize = true)
        {
            _logger.LogDebug("getting by mbid");
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            if (dc_artist == null)
            {
                _logger.LogDebug("failed to fetch by mbid");
                return StatusCode(404);
            }

            _logger.LogDebug("getting primary image");
            string imgUrl = dc_artist["images"].First(i => i.Value<string>("type") == "primary").Value<string>("uri");
            var imgResponse = await imgUrl.GetAsync();
            if (imgResponse.StatusCode != 200)
            {
                _logger.LogDebug("failed to fetch image uri");
                return StatusCode(imgResponse.StatusCode);
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
    }
}