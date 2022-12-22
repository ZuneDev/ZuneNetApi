using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Common;
using System;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("/v3.2/{culture}/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class PodcastController : Controller
    {
        private readonly ZuneNetContext _database;
        public PodcastController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpGet, Route("podcast/{tdid}")]
        public async Task<PodcastSeries> Details(Guid tdid)
        {
            var podcast = await Taddy.GetPodcastByTDID(tdid);
            await AddImagesToDatabase(_database, podcast);
            return podcast;
        }

        [HttpGet, Route("podcastCategories")]
        public Feed<Category> Categories()
        {
            Feed<Category> feed = new();

            foreach (var genre in Taddy.Genres)
                feed.Entries.Add(new()
                {
                    Id = genre.Key,
                    Title = genre.Value
                });

            return feed;
        }

        [NonAction]
        internal static async Task AddImagesToDatabase(ZuneNetContext database, PodcastSeries podcast)
        {
            if (podcast.Images.Count > 0)
            {
                // Add image ID
                var img = podcast.Images[0];
                var imgInst = img.Instances[0];
                var imgEntry = await database.AddImageAsync(imgInst.Url);
                img.Id = imgInst.Id = imgEntry.Id;
            }
        }
    }
}
