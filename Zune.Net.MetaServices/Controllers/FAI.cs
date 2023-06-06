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
        public async Task<ActionResult> Search(string SearchString, string resultTypeString, int maxNumberOfResults = 10) =>
            // var results = await MusicBrainz.MDARSearchAlbums(SearchString);
            // //resultTypeString album or artist
            // return results[0];
            resultTypeString switch
            {
                //{WMISFAIAlbumsQuery}, List<Album> Album
                "album" => Ok(await _wmis.SearchAlbumsAsync(SearchString, maxNumberOfResults)),// mdsr-cd
                                                                                               //  UIX Type: AlbumList, of Album
                "artist" => Ok("<METADATA><MDSR-CD><ReturnCode>SUCCESS</ReturnCode></MDSR-CD></METADATA>"),// mdsr-cd, but with slightly different elements
                                                                                                           //  UIX Type: ArtistList, of Artist
                "track" => Ok(),// WMISFAITracksQuery->TrackList (see WMISDATA.SCHEMA.XML)
                _ => NotFound(),
            };

        // example - http://metaservices.zune.net/ZuneFAI/GetAlbumDetailsFromAlbumId?albumId=8144290952437462496&locale=1033&volume=1
        // Also known as WMISFAIGetAlbumDetailsQuery in the UIX side
        // Looking for AlbumDetails - AKA mdar-cd
        [HttpPost("GetAlbumDetailsFromAlbumId")]
        [HttpGet("GetAlbumDetailsFromAlbumId")]
        [Produces("application/xml")]
        public async Task<ActionResult> MDARGetAlbumDetailsFromAlbumId(Int64 albumId, int locale, string volume, [FromBody] MdqRequestMetadata? request = null)
        {
            var response = await _wmis.GetMdarCdRequestFromInt64(albumId, volume, request!);
            if (request?.MdqCd?.MdqRequestId != null)
            {
                var mdqRequestId = request?.MdqCd?.MdqRequestId;
                if (string.IsNullOrEmpty(mdqRequestId))
                {
                    response.mdqRequestID = new Guid();
                }
                else
                {
                    response.mdqRequestID = new Guid(mdqRequestId);
                }
            }
            return Ok(response);
        }

        [HttpPost("SubmitAddFeedback")]
        public IActionResult SubmitAddFeedback()
        {
            return Ok();
        }
    }
}