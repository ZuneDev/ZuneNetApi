using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Podcast
{
    [Route("/v3.2/{culture}/podcast/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class PodcastController : Controller
    {
        [HttpGet, Route("{zid}")]
        public async Task<ActionResult> Details(Guid zid)
        {
            int amid = BitConverter.ToInt32(zid.ToByteArray().AsSpan(0, 4));
            var podcast = await Helpers.AppleMusic.Client.LookupPodcast(amid);
            var rss = await podcast.SourceUrl.GetStringAsync();
            return Content(rss, Atom.Constants.ATOM_MIMETYPE);
        }
    }
}
