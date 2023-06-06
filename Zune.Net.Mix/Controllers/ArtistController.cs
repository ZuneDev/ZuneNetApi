using System.Threading.Tasks;
using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Mix.Controllers
{
    [Route("/v4.0/{culture}/")]
    public class ArtistController : Controller
    {
        private ILogger _logger;
        public ArtistController(ILogger<ArtistController> logger)
        {
            _logger = logger;
        }

        [HttpGet("artist/{mbid}")]
        [Produces(Atom.Constants.ATOM_MIMETYPE)]
        public async Task<ActionResult<Feed<Artist>>> SimilarArtists(Guid mbid)
        {
            return await LastFM.GetSimilarArtistsByMBID(mbid);
        }

        [HttpGet("model/artist/{mbid}")]
        [Produces("application/xml")]
        public async Task<ActionResult<VectorEntry>> SimilarArtistsModel(Guid mbid)
        {
            var artistGenreIds = await MusicBrainz.GetArtistGenreIdsByArtistIdAsync(mbid);
            if(artistGenreIds.Count == 0)
            {
                _logger.LogInformation($"No genres were found for artistId: {mbid}");
                return NotFound();
            }

            _logger.LogInformation($"Returning {artistGenreIds.Count} genres for artistId: {mbid}");
            return Ok(new VectorEntry(mbid, artistGenreIds));
        }
    }
}
