using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Zune.Net.Helpers;
using Zune.Net.Helpers.AppleMusic;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("/v3.2/{culture}/podcastchart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController : Controller
    {
        [HttpGet, Route("podcasts")]
        public async Task<ActionResult<Feed<PodcastSeries>>> Podcasts(string culture)
        {
            int limit = 26;
            if (Request.Query.TryGetValue("chunkSize", out var chunkSizeVal))
                limit = int.Parse(chunkSizeVal[0]);

            return await AppleMusicClient.GetPodcastChart(limit);
            return await Listen.GetBestPodcastsByLNGenre();
        }
    }
}
