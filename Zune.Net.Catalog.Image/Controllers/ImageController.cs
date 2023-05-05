using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Image.Controllers
{
    [Route("/v{version:decimal}/{culture}/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ImageController : Controller
    {
        private static readonly Uri defaultImage = new Uri("https://hellofromseattle.com/wp-content/uploads/sites/6/2020/03/Zune-Basic.png");
        private static readonly ConcurrentDictionary<int, JObject> dcArtistCache = new();
        private static readonly int[] caaSupportedSizes = new[] { 250, 500, 1200 };

        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;
        public ImageController(ZuneNetContext database, ILogger<ImageController> logger)
        {
            _database = database;
            _logger = logger;
        }

        [HttpGet, Route("image/{id}")]
        public async Task<IActionResult> Image(Guid id)
        {
            string? imageUrl = null;

            (var idA, var idB, var idC) = id.GetGuidParts();
            var imageEntry = await _database.GetImageEntryAsync(id);

            if (imageEntry != null)
            {
                imageUrl = imageEntry.Url;
            }
            else if (idC == 0)
            {
                int dcid = unchecked((int)idA);

                // Get or update cached artist
                if (!dcArtistCache.TryGetValue(dcid, out var dc_artist))
                {
                    // Artist not in cache
                    dc_artist = await Discogs.GetDCArtistByDCID(dcid);
                    dcArtistCache.AddOrUpdate(dcid, _ => dc_artist, (_, _) => dc_artist);
                }

                // Get URL for requested image
                var images = dc_artist.Value<JArray>("images");
                if (images != null && images.Count > idB)
                {
                    var image = images[idB];
                    imageUrl = image.Value<string>("uri");
                }
            }
            else
            {
                // The Cover Art Archive API supports sizes of 250, 500, and 1200
                int requestedWidth = 500;
                if (Request.Query.TryGetValue("width", out var widthValues) && widthValues.Count > 0)
                    int.TryParse(widthValues[0], out requestedWidth);

                int width = caaSupportedSizes.MinBy(x => Math.Abs(x - requestedWidth));
                imageUrl = $"https://coverartarchive.org/release/{id}/front-{width}";
            }

            if(Request.Query.TryGetValue("resize", out var resize))
            {
                if(bool.TryParse(resize, out var _resize))
                {
                    if(_resize) // this flag basically states the zune is expecting the image to be the payload, so we hackily respond with that
                    {
                        _logger.LogInformation("streaming the image to the zune now!");
                        // Probably should pull the contenttype value out of the query...
                        using var client = new HttpClient();
                        var result = await client.GetAsync(imageUrl);
                        if(!result.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Falling back to the default image, failed to resolve actual artwork");
                            result = await client.GetAsync(defaultImage);
                        }
                        return new HttpResponseMessageResult(result);
                    }
                }
            }

            // Request the image from the API and forward it to the Zune software
            return imageUrl != null
                ? Redirect(imageUrl)
                : NotFound();
        }

        // i.e. http://image.catalog.zune.net/v3.0/en-US/music/track/f32bb0ab-59d6-4620-b239-e86dc68647a4/albumImage?width=240&height=240&resize=true

        [HttpGet, Route("music/{imageKind}/{id}/{type}")]
        public async Task<IActionResult> ArtistImage(Guid id, string type)
        {
            Uri? imageUrl = null;

            if (type == "primaryImage")
            {
                try
                {
                    _logger.LogInformation($"Getting image for Artist");
                    (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(id);

                    imageUrl = new Uri(dc_artist.Value<JArray>("images")?
                        .FirstOrDefault(i => i.Value<string>("type") == "primary")?
                        .Value<string>("uri"));
                } catch 
                {
                    _logger.LogInformation($"Failed to get by Artist ID, failing over to albumImage lookup method");
                    type = "albumImage";
                }

            }
            if(type == "albumImage")
            {
                try
                {
                    _logger.LogInformation($"Getting image from RecordingID");
                    var albumId = MusicBrainz.GetAlbumByRecordingId(id).Id;
                    _logger.LogInformation($"Got ID{albumId}");
                    //e.g.: http://coverartarchive.org/release/93c488f3-1739-455d-a609-85b846b45344/front-250
                    imageUrl = new Uri($"http://coverartarchive.org/release/{albumId}/front-250");
                } catch (Exception e) 
                {
                    _logger.LogError(e, "Failed to get AlbumID");
                }

                if(imageUrl == null)
                {
                    _logger.LogInformation($"Getting image failed, falling back");
                    imageUrl = new Uri($"http://coverartarchive.org/release/{id}/front-250");
                }

                _logger.LogInformation($"Getting image from: {imageUrl.AbsoluteUri}");

                using var client = new HttpClient();
                var result = await client.GetAsync(imageUrl);
                if(!result.IsSuccessStatusCode)
                {
                    result = await client.GetAsync(defaultImage);
                }
                return new HttpResponseMessageResult(result);
            }

            return imageUrl != null
                ? Redirect(imageUrl.AbsoluteUri)
                : NotFound();
        }
    }
}
