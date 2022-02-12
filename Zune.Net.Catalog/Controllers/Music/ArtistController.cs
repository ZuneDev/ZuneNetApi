using Atom.Xml;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Catalog.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult<Feed<Artist>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchArtists(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public ActionResult<Artist> Details(Guid mbid)
        {
            return MusicBrainz.GetArtistByMBID(mbid);
        }

        [HttpGet, Route("{mbid}/tracks")]
        public ActionResult<Feed<Track>> Tracks(Guid mbid)
        {
            if (!Request.Query.TryGetValue("chunkSize", out var chunkSizeStrs) || chunkSizeStrs.Count != 1)
                return BadRequest();

            return MusicBrainz.GetArtistTracksByMBID(mbid, Request.Path, int.Parse(chunkSizeStrs[0]));
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid)
        {
            return MusicBrainz.GetArtistAlbumsByMBID(mbid, Request.Path);
        }

        [HttpGet, Route("{mbid}/primaryImage")]
        public async Task<ActionResult> Image(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);

            string imgUrl = dc_artist["images"].First(i => i.Value<string>("type") == "primary").Value<string>("uri");
            var imgResponse = await imgUrl.GetAsync();
            if (imgResponse.StatusCode != 200)
                return StatusCode(imgResponse.StatusCode);
            return File(await imgResponse.GetStreamAsync(), "image/jpeg");
        }
    }
}
