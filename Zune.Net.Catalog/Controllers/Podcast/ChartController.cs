using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zune.DB;
using Zune.Net.Features;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("/podcastchart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController(ZuneNetContext database) : Controller
    {
        [HttpGet, Route("podcasts")]
        public async Task<ActionResult<Feed<PodcastSeries>>> Podcasts([FromQuery] int chunkSize = 50)
        {
            var cultureFeature = HttpContext.Features.Get<ICultureFeature>();
            var region = cultureFeature.Region;
            
            var feed = await Listen.GetBestPodcasts(region, limit: chunkSize);

            await Parallel.ForEachAsync(feed.Entries, async (podcast, token) =>
            {
                try
                {
                    // Search on Taddy for RSS URL
                    var td_pod = await Taddy.GetMinimalPodcastInfo(podcast.Title.Value);
                    podcast.Id = td_pod.Id.ToString();
                    podcast.FeedUrl = td_pod.RssUrl;
                    podcast.Content = td_pod.Description;

                    await PodcastController.AddImagesToDatabase(database, podcast);
                }
                catch { }
            });

            return feed;
        }
    }
}
