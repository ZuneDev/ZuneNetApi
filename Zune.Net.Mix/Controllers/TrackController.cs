using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.Net.Shared.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Mix.Controllers
{
    [Route("/v4.0/{culture}/track/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class TrackController : Controller
    {

        [HttpGet, Route("{mbid}/similarTracks")]
        public async Task<ActionResult<Feed<Track>>> SimilarTracks(Guid mbid)
        {
            return await LastFM.GetSimilarTracksByMBID(mbid);
        }
    }
}
