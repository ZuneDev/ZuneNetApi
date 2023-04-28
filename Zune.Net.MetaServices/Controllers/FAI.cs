using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Net.MetaServices.DomainModels.Endpoints;

namespace Zune.Net.MetaServices.Controllers
{
    [Route("/ZuneFAI/")]
    public class FAI : Controller
    {
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
                    return Ok(await WMIS.SearchAlbums(SearchString));
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
        [HttpGet("GetAlbumDetailsFromAlbumId")]
        [Produces("application/xml")]
        public async Task<ActionResult> MDARGetAlbumDetailsFromAlbumId()
        {
            return Ok();
        }
    }
}