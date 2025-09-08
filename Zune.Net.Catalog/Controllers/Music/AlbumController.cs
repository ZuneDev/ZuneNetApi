using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using HtmlAgilityPack;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/album/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class AlbumController : Controller
    {

        [HttpGet, Route("")]
        public ActionResult<Feed<Album>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchAlbums(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid:guid}")]
        public ActionResult<Album> Details(Guid mbid)
        {
            return MusicBrainz.GetAlbumByMBID(mbid);
        }

        [HttpGet, Route("{mbid:guid}/review")]
        public async Task<ActionResult<Album>> Review(Guid mbid, IdMapper idMapper)
        {
            var releaseGroupResults = await MusicBrainz._query
                .FindReleaseGroupsAsync($"reid:{mbid}", simple: false, limit: 1);
            
            var releaseGroupMbid = releaseGroupResults.Results.FirstOrDefault()?.Item.Id;
            if (releaseGroupMbid is null)
                return BadRequest();
            
            var albumIdMap = await idMapper.GetAlbumIdsByMbidAsync(releaseGroupMbid.Value);

            var alid = albumIdMap?.AllMusic;
            if (alid is null)
                return NotFound();

            try
            {
                await using var reviewAjaxStream = await $"https://www.allmusic.com/album/{alid}/reviewAjax"
                    .WithHeader("Referer", "https://www.allmusic.com")
                    .GetStreamAsync();
                
                HtmlDocument doc = new();
                doc.Load(reviewAjaxStream);

                var reviewNode = doc.DocumentNode.SelectSingleNode("//div[@id='review']");
                var headerNode = reviewNode!.SelectSingleNode("h3");
                var paragraphNode = reviewNode!.SelectSingleNode("p");
                
                var nameSegments = headerNode!.InnerText.Split(" by ");
                var authorName = nameSegments[^1];

                Review review = new()
                {
                    Text = paragraphNode?.InnerText,
                    Author = authorName,
                };

                return new Album
                {
                    Review = review,
                };
            }
            catch (FlurlHttpException ex)
            {
                return StatusCode(ex.StatusCode ?? 500, ex.Message);
            }
        }
    }
}
