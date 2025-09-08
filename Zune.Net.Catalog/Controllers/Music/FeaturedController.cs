using System.Threading.Tasks;
using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music;

[Route("/music/featured/")]
[Produces(Atom.Constants.ATOM_MIMETYPE)]
public class FeaturedController : Controller
{
    [HttpGet, Route("albums")]
    public async Task<ActionResult<Feed<Album>>> Albums()
    {
        var (apiVersion, culture) = this.GetCurrentVersionAndCulture();

        var albums = await ListenBrainz.ExploreFreshReleases(7, "release_date", false, 20,
            caaReleaseMbid => $"http://image.catalog.zunes.me/v{apiVersion}/{culture}/image/{caaReleaseMbid}");
        
        return new Feed<Album>
        {
            Title = "Fresh Releases",
            Author = new Author
            {
                Name = "ListenBrainz",
                Url = "https://listenbrainz.org/"
            },
            Links = [
                new Link
                {
                    Href = "https://listenbrainz.org/explore/fresh-releases/",
                    Relation = "self",
                }
            ],
            Entries = albums
        };
    }
}