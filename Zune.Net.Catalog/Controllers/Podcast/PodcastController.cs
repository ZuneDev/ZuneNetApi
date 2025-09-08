using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Flurl.Http;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class PodcastController(ZuneNetContext database) : Controller
    {
        [HttpGet, Route("podcast")]
        public async Task<IActionResult> Search([FromQuery(Name = "q")] string query,
            [FromQuery] string url)
        {
            if (url is not null)
            {
                // TODO: See https://zunepodcasts.net/
                await using var podcastStream = await url.GetStreamAsync();
                return File(podcastStream, "application/rss+xml");
            }
            
            var feed = await Listen.SearchPodcasts(query);
            foreach (var podcast in feed.Entries)
                await AddImagesToDatabase(database, podcast);

            return Ok(feed);
        }

        [HttpGet, Route("podcast/{lnid}")]
        public async Task<PodcastSeries> Details(Guid lnid)
        {
            var podcast = await Listen.GetPodcast(lnid.ToString("N"));
            
            // Search on Taddy for RSS URL
            var td_pod = await Taddy.GetMinimalPodcastInfo(podcast.Title.Value);
            podcast.Id = td_pod.Id.ToString();
            podcast.FeedUrl = td_pod.RssUrl;
            
            await AddImagesToDatabase(database, podcast);
            return podcast;
        }

        [HttpGet, Route("podcastCategories")]
        public Feed<Category> Categories()
        {
            // TODO: Narrow the categories down to a few, not 110
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
            if (podcast is not { Images.Count: > 0 })
                return;

            // Add image ID
            var img = podcast.Images[0];
            var imgInst = img.Instances[0];
            var imgEntry = await database.AddImageAsync(imgInst.Url);
            img.Id = imgInst.Id = imgEntry.Id;
        }
    }
}
