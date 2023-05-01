using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Net.MetaServices.DomainModels.Endpoints;
using Zune.Net.MetaServices.DomainModels.MdqCd;

namespace Zune.Net.MetaServices.Controllers
{
    [Route("/ZuneFAI/")]
    public class FAI : Controller
    {
        private readonly WMIS _wmis;
        private readonly ILogger _logger;
        public FAI(WMIS wmis, ILogger<FAI> logger)
        {
            _wmis = wmis;
            _logger = logger;
        }
        [HttpGet("Search")]
        [Produces("application/xml")]
        public async Task<ActionResult> Search(string SearchString, string resultTypeString)
        {
            // var results = await MusicBrainz.MDARSearchAlbums(SearchString);
            // //resultTypeString album or artist
            // return results[0];
            switch (resultTypeString)
            {
                case "album":
                    // if SearchString, not specific, but artistId is specific to artist
                    // return Ok(new AlbumList{
                    //   ReturnCode = "SUCCESS",
                    //   Items = await MusicBrainz.SearchForAlbums(SearchString)
                    // });
                    return Ok(await _wmis.SearchAlbumsAsync(SearchString));
                // case "artist":
                //     return Ok(new ArtistList(){
                //       ReturnCode = "SUCCESS",
                //       Items = new List<Xml.MDAR.Artist>()
                //     });
                // case "track":
                //     return Ok(new TrackList()
                //     {
                //         ReturnCode = "SUCCESS",
                //         Items = new List<Xml.MDAR.Track>()
                //     });
            }
            return NotFound();
        }

        // Also known as WMISFAIGetAlbumsByArtistQuery in the UIX side
        [HttpPost("GetAlbumDetailsFromAlbumId")]
        [Produces("application/xml")]
        public async Task<ActionResult> MDARGetAlbumDetailsFromAlbumId([FromBody]MdqRequestMetadata request, int albumId, int locale, int volume)
        {
            _logger.LogInformation($"MDQ-CD request for Album: {request.MdqCd.Album.AlbumTitle.Text} by {request.MdqCd.Album.AlbumArtist.Text}");
            foreach(var track in request.MdqCd.Tracks)
            {
                _logger.LogInformation($"Track: {track.TrackNumber} - {track.Title.Text}: RequestID: {track.trackRequestId}");
            }
            return Ok();
        }
    }
}