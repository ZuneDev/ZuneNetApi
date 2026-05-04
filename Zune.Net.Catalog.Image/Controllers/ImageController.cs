using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Net.Identifiers;

namespace Zune.Net.Catalog.Image.Controllers
{
    [Route("/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ImageController(ZuneNetContext database, IdMapper idMapper) : Controller
    {
        private static readonly ConcurrentDictionary<int, JObject> DcArtistCache = new();
        private static readonly int[] CaaSupportedSizes = [250, 500, 1200];

        [HttpGet, Route("image/{id:guid}")]
        public async Task<IActionResult> Image(Guid id,
            [FromQuery] bool resize = false, [FromQuery(Name = "width")] int? requestedWidth = null,
            [FromQuery] string contentType = MediaTypeNames.Image.Jpeg)
        {
            string? imageUrl = null;

            var (idA, idB, idC) = id.GetGuidParts();
            var imageEntry = await database.GetImageEntryAsync(id);

            if (imageEntry is not null)
            {
                imageUrl = imageEntry.Url;
            }
            else if (idC == 0)
            {
                var dcid = unchecked((int)idA);

                // Get or update cached artist
                if (!DcArtistCache.TryGetValue(dcid, out var dcArtist))
                {
                    // Artist not in cache
                    dcArtist = await Discogs.GetDCArtistByDCID(dcid);
                    DcArtistCache.AddOrUpdate(dcid, _ => dcArtist, (_, _) => dcArtist);
                }

                // Get URL for requested image
                var images = dcArtist.Value<JArray>("images");
                if (images != null && images.Count > idB)
                    imageUrl = images[idB].Value<string>("uri");
            }
            else
            {
                // Find nearest size supported by Cover Art Archive API
                var targetWidth = requestedWidth ?? 500;
                var width = CaaSupportedSizes.MinBy(x => Math.Abs(x - targetWidth));
                imageUrl = $"https://coverartarchive.org/release/{id}/front-{width}";
            }

			return await this.GetAndResizeImageAsync(imageUrl, resize, requestedWidth, contentType);
        }

        [HttpGet, Route("music/artist/{id}/{type}")]
        public async Task<IActionResult> ArtistImage(string id, string type,
            [FromQuery] bool resize = false, [FromQuery(Name = "width")] int? requestedWidth = null,
            [FromQuery] string contentType = MediaTypeNames.Image.Jpeg)
        {
            string? imageUrl = null;

            if (type == "primaryImage")
            {
                var mbid = Guid.Parse(id);
                var dcArtist = await Discogs.GetDCArtistByMBID(mbid, idMapper);

                imageUrl = dcArtist.Value<JArray>("images")?
                    .FirstOrDefault(i => i.Value<string>("type") == "primary")?
                    .Value<string>("uri");
            }

            return await this.GetAndResizeImageAsync(imageUrl, true, requestedWidth, contentType);
        }
    }
}
