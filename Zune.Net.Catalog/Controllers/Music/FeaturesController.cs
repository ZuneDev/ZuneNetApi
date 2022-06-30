using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/features/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class FeaturesController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult<Feed<AlbumCollection>> Features()
        {
            var updated = DateTime.Now;

            Feed<AlbumCollection> feed = new()
            {
                Title = "Features",
                Author = new()
                {
                    Name = "Joshua Askharoun",
                    Url  = "https://josh.askharoun.com"
                },
                Id = "features",
                Updated = updated,
                Entries = new()
                {
                    new AlbumCollection
                    {
                        Id = "dddddddd-dddd-dddd-dddd-dddddddddddd",
                        Updated = updated,
                        Index = 1,
                        Link = new()
                        {
                            Type = "AlbumCollection",
                            Target = "dddddddd-aaaa-dddd-dddd-dddddddddddd"
                        },
                        Title = "List of features",
                        Content = "This is a test of features",
                        Links = new()
                        {
                            new()
                            {
                                Updated = updated,
                                Href = "/v3.2/en-US/music/album/7a1d2768-1007-4ea3-845f-4a5e4790a25b",
                                Relation = "alternate",
                                Type = Atom.Constants.ATOM_MIMETYPE
                            }
                        },
                        EditorialItems = new()
                        {
                            new Feature
                            {
                                Id = "addddddd-bbbb-dddd-dddd-dddddddddddd",
                                Title = "Album",
                                Text = "OKAY",
                                Link = new()
                                {
                                    Type = "Album",
                                    Target = "7a1d2768-1007-4ea3-845f-4a5e4790a25b"
                                },
                                SequenceNumber = 1,
                                BackgroundImage = new()
                                {
                                    Id = Guid.Parse("dddddddd-aaaa-aaaa-dddd-dddddddddddd"),
                                    Instances = new()
                                    {
                                        new()
                                        {
                                            Id = Guid.Parse("dddddddd-aaaa-aaaa-dddd-dddddddddddA"),
                                            Url = "https://wallpaperhub.app/api/v1/get/7786/0/1080p"
                                        }
                                    },
                                },
                                Image = new()
                                {
                                    Id = Guid.Parse("dddddddd-aaaa-bbbb-dddd-dddddddddddd"),
                                    Instances = new()
                                    {
                                        new()
                                        {
                                            Id = Guid.Parse("dddddddd-aaaa-bbbb-dddd-ddddddddddda"),
                                            Url = "https://www.wired.com/wp-content/uploads/2015/09/zune-story.jpg"
                                        }
                                    }
                                }
                            }
                        },
                        Albums = new()
                        {
                            new Album
                            {
                                Id = "7a1d2768-1007-4ea3-845f-4a5e4790a25b",
                                Actionable = true,
                                PrimaryArtist = new()
                                {
                                    Title = "Tokyo Machine",
                                    Id = new("63c662d7-cfa5-493b-9ad4-6f428438f2fd")
                                },
                                Explicit = false,
                                Title = "OKAY",
                            }
                        }
                    }
                }
            };

            return feed;
        }
    }
}
