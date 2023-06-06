using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Net.Mix.DomainModel;

namespace Zune.Net.Mix.Controllers
{
    [Route("/v3.0/model/album/")]
    [Route("/v4.0/{culture}/model/album/")]
    [Produces("application/xml")]
    public class AlbumController : Controller
    {
        private ILogger _logger;
        public AlbumController(ILogger<AlbumController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{mbid}")]
        public async Task<ActionResult<AlbumModel>> SimilarAlbum(Guid mbid)
        {
            // First, get all of the tracks on an album MBID
            var trackList = await MusicBrainz.GetTracksByAlbumMbidAsync(mbid);

            // now, get all the genreIDs
            var genreIds = await MusicBrainz.GetGenreIdsByAlbumMbidAsync(mbid);

            // If we don't have enough data, 404. The API will eventually rerequest this.
            if(genreIds.Count == 0 || trackList.Count == 0)
            {
                _logger.LogInformation($"No genre data available for albumId{mbid}");
                return NotFound();
            }

            var response = new AlbumModel();

            foreach(var trackId in trackList)
            {
                response.Entry.Add(new(trackId, genreIds));
            }

            _logger.LogInformation($"Returning {genreIds.Count} genres and {trackList.Count} tracks for albumId: {mbid}");

            return Ok(response);
        }
    }
}
