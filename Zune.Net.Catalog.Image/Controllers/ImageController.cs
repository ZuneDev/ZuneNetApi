using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        private readonly ZuneNetContext _database;
        public ImageController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpGet, Route("image/{id}")]
        public async Task<IActionResult> Image(Guid id)
        {
            string imageUrl = "";

            //Console.WriteLine(id.ToString());

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
                    imageUrl = image.Value<string>("uri")??"";
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
            MemoryStream memStream = new MemoryStream();
            if (imageUrl != "")
            {
                memStream = await httpToMemSreamAsync(imageUrl);
            }

            // Request the image from the API and forward it to the Zune software
            return (memStream.Length > 0)
                ? File(memStream.ToArray(), "image/jpg")
                : NotFound();
        }

        public async Task<MemoryStream> httpToMemSreamAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Zune-4.8");
            MemoryStream imgMemStream = new MemoryStream();
            Stream imgStream = new MemoryStream();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    imgStream = response.Content.ReadAsStream();
                }
            }
            catch (HttpRequestException w)
            {
                Console.WriteLine(w.ToString());
            }
            imgStream.CopyTo(imgMemStream);
            return imgMemStream;
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
