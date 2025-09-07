using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atom.Xml;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Zune.Net.Features;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music;

[Route("/music/featured/")]
[Produces(Atom.Constants.ATOM_MIMETYPE)]
public class FeaturedController : Controller
{
    [HttpGet, Route("albums")]
    public async Task<ActionResult<Feed<Album>>> Albums()
    {
        var cultureFeature = HttpContext.Features.Get<ICultureFeature>();
        var culture = cultureFeature?.CultureString ?? "en-US";
        
        var apiVersionFeature = HttpContext.Features.Get<IApiVersionFeature>();
        var apiVersion = apiVersionFeature?.Version ?? "3.2";
        
        var response = await "https://api.listenbrainz.org/1/explore/fresh-releases"
            .SetQueryParam("days", 7)
            .SetQueryParam("sort", "release_date")
            .SetQueryParam("future", false)
            .WithHeader("User-Agent", "Zune/4.8")
            .GetJsonAsync<JObject>();

        var lb_releases = response["payload"]!["releases"]!;
        List<Album> albums = new(20);

        foreach (var lb_release in lb_releases.Take(20))
        {
            var artistName = lb_release.Value<string>("artist_credit_name");
            var artistMbids = lb_release["artist_mbids"]!.ToObject<List<Guid>>();
            var releaseMbid = lb_release.Value<string>("release_mbid");
            var releaseName = lb_release.Value<string>("release_name");
            var caaReleaseMbid = lb_release.Value<string>("caa_release_mbid");
            var releaseDate = lb_release.Value<DateTime>("release_date");
            var listenCount = lb_release.Value<int>("listen_count");
            
            // ListenBrainz combines all credited artists into the name field in alphabetical order,
            // which doesn't necessarily match the order of the artist MBIDs

            Album album = new()
            {
                Title = releaseName,
                Id = releaseMbid,
                ReleaseDate = releaseDate,
                PrimaryArtist = new MiniArtist
                {
                    Id = artistMbids[0],
                    Title = artistName,
                },
                Popularity = listenCount,
            };

            if (caaReleaseMbid is not null)
            {
                album.Images =
                [
                    new Image
                    {
                        Id = new Guid(caaReleaseMbid),
                        Instances =
                        [
                            new ImageInstance
                            {
                                Id = new Guid(caaReleaseMbid),
                                Url = $"http://image.catalog.zunes.me/v{apiVersion}/{culture}/image/{caaReleaseMbid}"
                            }
                        ]
                    }
                ];
            }
            
            albums.Add(album);
        }
        
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
            Entries = albums.OrderByDescending(a => a.ReleaseDate).ToList()
        };
    }
}