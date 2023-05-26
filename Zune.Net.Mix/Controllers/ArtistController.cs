using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Mix.Controllers
{
    [Route("/v4.0/{culture}/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController : Controller
    {

        [HttpGet, Route("{mbid}")]
        public async Task<ActionResult<Feed<Artist>>> SimilarArtists(Guid mbid)
        {
            return await LastFM.GetSimilarArtistsByMBID(mbid);
        }
    }
}
