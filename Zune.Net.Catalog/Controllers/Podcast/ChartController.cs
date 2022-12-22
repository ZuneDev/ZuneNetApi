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

        [HttpGet, Route("podcasts")]
        public async Task<ActionResult<Feed<PodcastSeries>>> Podcasts(string culture)
        {
            int limit = 26;
            if (Request.Query.TryGetValue("chunkSize", out var chunkSizeVal))
                limit = int.Parse(chunkSizeVal[0]);

            var feed = await Listen.GetBestPodcasts(culture[(culture.IndexOf('-') + 1)..], limit: limit);

            foreach (var entry in feed.Entries)
            {
                if (entry.Images.Count > 0)
                {
                    // Add image ID
                    var img = entry.Images[0];
                    var imgInst = img.Instances[0];
                    var imgEntry = await _database.AddImageAsync(imgInst.Url);
                    img.Id = imgInst.Id = imgEntry.Id;
                }

                // Search on Taddy for RSS URL
                var td_pod = await Taddy.GetMinimalPodcastInfo(entry.Title.Value);
                entry.Id = td_pod.Id.ToString();
                entry.FeedUrl = td_pod.RssUrl;
                entry.Content = td_pod.Description;
            }

            return feed;
        }
    }
}
