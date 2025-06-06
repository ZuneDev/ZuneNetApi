using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.DataProviders;
using Zune.DB;

namespace Zune.Net.Catalog.Image.Controllers
{
    [Route("/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ImageController(ZuneNetContext database, AggregatedMediaProvider mediaProvider) : Controller
    {
        private static readonly int[] caaSupportedSizes = new[] { 250, 500, 1200 };

        [HttpGet, Route("image/{id}")]
        public async Task<IActionResult> Image(Guid id)
        {
            string? imageUrl = null;

            var imageEntry = await database.GetImageEntryAsync(id);

            if (imageEntry is not null)
            {
                imageUrl = imageEntry.Url;
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
                var artistId = new MediaId(id, KnownMediaSources.MusicBrainz, MediaType.Artist);
                imageUrl = await mediaProvider.GetArtistPrimaryImage(artistId);
            }

            return imageUrl != null
                ? Redirect(imageUrl)
                : NotFound();
        }
    }
}
