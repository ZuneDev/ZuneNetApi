using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;

namespace Zune.Net.Catalog.Image.Controllers
{
    [Route("/v3.2/{culture}/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ImageController : Controller
    {
        private static readonly ConcurrentDictionary<int, JObject> dcArtistCache = new();
        private static readonly int[] caaSupportedSizes = new[] { 250, 500, 1200 };
        private static readonly string zeroStr = new('0', 12);

        private readonly ZuneNetContext _database;
        public ImageController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpGet, Route("image/{id}")]
        public async Task<IActionResult> Image(string id)
        {
            string? imageUrl = null;
            if (id.EndsWith(zeroStr))
            {
                int dcid = int.Parse(id[..8], NumberStyles.HexNumber);
                short imgIdx = short.Parse(id[9..13], NumberStyles.HexNumber);

                // Get or update cached artist
                if (!dcArtistCache.TryGetValue(dcid, out var dc_artist))
                {
                    // Artist not in cache
                    dc_artist = await Discogs.GetDCArtistByDCID(dcid);
                    dcArtistCache.AddOrUpdate(dcid, _ => dc_artist, (_, _) => dc_artist);
                }

                // Get URL for requested image
                var images = dc_artist.Value<JArray>("images");
                if (images != null && images.Count > imgIdx)
                {
                    var image = images[imgIdx];
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

            // Request the image from the API and forward it to the Zune software
            return imageUrl != null
                ? Redirect(imageUrl)
                : NotFound();
        }

        [HttpGet, Route("music/artist/{id}/{type}")]
        public async Task<IActionResult> ArtistImage(string id, string type)
        {
            string? imageUrl = null;

            if (type == "primaryImage")
            {
                Guid mbid = Guid.Parse(id);
                (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);

                imageUrl = dc_artist.Value<JArray>("images")?
                    .FirstOrDefault(i => i.Value<string>("type") == "primary")?
                    .Value<string>("uri");
            }

            return imageUrl != null
                ? Redirect(imageUrl)
                : NotFound();
        }
    }
}
