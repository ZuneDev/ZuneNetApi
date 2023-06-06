using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("/v3.2/{culture}/podcastchart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController : Controller
    {
        private readonly ZuneNetContext _database;
        public ChartController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpGet("podcasts")]
        public async Task<ActionResult<Feed<PodcastSeries>>> Podcasts(string culture)
        {
            int limit = 26;
            if (Request.Query.TryGetValue("chunkSize", out var chunkSizeVal))
                limit = int.Parse(chunkSizeVal[0]);

            var feed = await Listen.GetBestPodcasts(culture[(culture.IndexOf('-') + 1)..], limit: limit);

            foreach (var podcast in feed.Entries)
            {
                await PodcastController.AddImagesToDatabase(_database, podcast);

                // Search on Taddy for RSS URL
                var td_pod = await Taddy.GetMinimalPodcastInfo(podcast.Title.Value);
                podcast.Id = td_pod.Id.ToString();
                podcast.FeedUrl = td_pod.RssUrl;
                podcast.Content = td_pod.Description;
            }

            return feed;
        }
    }
}
