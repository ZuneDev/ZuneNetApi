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
        public async Task<ActionResult> Search(string SearchString, string resultTypeString, int maxNumberOfResults = 10)
        {
            // var results = await MusicBrainz.MDARSearchAlbums(SearchString);
            // //resultTypeString album or artist
            // return results[0];
            switch (resultTypeString)
            {
                case "album": //{WMISFAIAlbumsQuery}, List<Album> Album
                    // mdsr-cd
                    //  UIX Type: AlbumList, of Album
                    return Ok(await _wmis.SearchAlbumsAsync(SearchString, maxNumberOfResults));
                case "artist":
                    // mdsr-cd, but with slightly different elements
                    //  UIX Type: ArtistList, of Artist
                    return Ok("<METADATA><MDSR-CD><ReturnCode>SUCCESS</ReturnCode></MDSR-CD></METADATA>");
                case "track":
                    return Ok();
            }
            return NotFound();
        }

        // example - http://metaservices.zune.net/ZuneFAI/GetAlbumDetailsFromAlbumId?albumId=8144290952437462496&locale=1033&volume=1
        // Also known as WMISFAIGetAlbumDetailsQuery in the UIX side
        // Looking for AlbumDetails - AKA mdar-cd
        [HttpPost("GetAlbumDetailsFromAlbumId")]
        [Produces("application/xml")]
        public async Task<ActionResult> MDARGetAlbumDetailsFromAlbumId([FromBody]MdqRequestMetadata request, Int64 albumId, int locale, int volume)
        {
            return Ok(await _wmis.GetMdarCdRequestFromInt64(albumId, volume));
        }
    }
}