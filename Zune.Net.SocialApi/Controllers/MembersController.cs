using Atom;
using Atom.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Zune.Xml.SocialApi;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Zune.SocialApi.Controllers
{
    [Route("/{locale}/members/{member}/{action=Info}")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        public async Task<IActionResult> Info(string member)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Member
            {
                Namespace = "http://schemas.zune.net/profiles/2008/01",
                Links =
                {
                    new Link
                    {
                        Relation = "self",
                        Type = "application/atom+xml",
                        Href = requestUrl
                    },
                    new Link
                    {
                        Relation = "related",
                        Type = "application/atom+xml",
                        Href = requestUrl + "/friends",
                        Title = "friends"
                    },
                },
                Updated = DateTime.UtcNow.ToString("O"),
                Title = new Content
                {
                    Type = "text",
                    Value = member
                },
                Content = new Content
                {
                    Type = "html",
                    Value = @"<p>This is HTML code being displayed by the Zune software on my social page.</p>"
                },
                Author = new Author
                {
                    Name = member,
                    Url = "http://social.zune.net/member/" + member
                },
                Id = "894090e7-b88e-4e3a-9ff8-eea48848638e",
                Rights = "Copyright (c) Microsoft Corporation.  All rights reserved.",

                PlayCount = 206,
                ZuneTag = member,
                DisplayName = "",
                Status = "Reviving the Zune social",
                Bio = "A computer science student at Texas A&M Univserity that can't help but bring back dead Microsoft products.",
                Location = "College Station, Texas",
                Images =
                {
                    new Link
                    {
                        Relation = "enclosure",
                        Href = "http://web.archive.org/web/20110510042505if_/http://tiles.zune.net/xweb/lx/pic/64x64_tile.jpg",
                        Title = "usertile"
                    },
                    new Link
                    {
                        Relation = "enclosure",
                        Href = "http://cache-tiles.zune.net/tiles/background/" + member,
                        Title = "background"
                    },
                }
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", "http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new(typeof(Member));

            Console.Out.WriteLine("<!-- " + requestUrl + " -->");
            serializer.Serialize(Console.Out, feed, ns);
            Console.Out.Write("\n\n");

            Stream body = new MemoryStream();
            serializer.Serialize(body, feed, ns);
            body.Flush();
            body.Position = 0;
            return File(body, "application/xml");
        }

        public async Task<IActionResult> Friends(string member)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Feed
            {
                Namespace = Constants.ZUNE_PROFILES_NAMESPACE,
                Links =
                {
                    new Link
                    {
                        Relation = "self",
                        Type = "application/atom+xml",
                        Href = requestUrl
                    }
                },
                Updated = DateTime.UtcNow.ToString("O"),
                Title = new Content
                {
                    Type = "text",
                    Value = member + "'s Friends"
                },
                Author = new Author
                {
                    Name = member,
                    Url = "http://social.zune.net/member/" + member
                },
                Id = "894090e7-b88e-4e3a-9ff8-eea48848638e",
                Rights = "Copyright (c) Microsoft Corporation.  All rights reserved."
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", "http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new(typeof(Feed));

            Console.Out.WriteLine("<!-- " + requestUrl + " -->");
            serializer.Serialize(Console.Out, feed, ns);
            Console.Out.Write("\n\n");

            Stream body = new MemoryStream();
            serializer.Serialize(body, feed, ns);
            body.Flush();
            body.Position = 0;
            return File(body, "application/xml");

            //var doc = new XmlDocument();
            //var nsManager = new XmlNamespaceManager(doc.NameTable);
            //nsManager.AddNamespace("a", "http://www.w3.org/2005/Atom");

            //var feed = doc.CreateElement("a", "feed", null);
            //feed.SetAttribute("xmlns", "http://schemas.zune.net/profiles/2008/01");

            //var link = doc.CreateElement("a", "link", null);
            //link.SetAttribute("rel", "self");
            //link.SetAttribute("type", "application/atom+xml");
            //link.SetAttribute("href", Request.Path);
            //feed.AppendChild(link);

            //var updated = doc.CreateElement("a", "updated", null);
            //updated.InnerText = DateTime.UtcNow.ToString("O");
            //feed.AppendChild(updated);

            //var title = doc.CreateElement("a", "title", null);
            //title.SetAttribute("type", "text");
            //title.InnerText = member + "'s Friends";
            //feed.AppendChild(title);

            //var author = doc.CreateElement("a", "author", null);
            //var authorName = doc.CreateElement("a", "name", null);
            //authorName.InnerText = member;
            //var authorUri = doc.CreateElement("a", "uri", null);
            //authorUri.InnerText = "http://social.zune.net/member/" + member;
            //author.AppendChild(authorName);
            //author.AppendChild(authorUri);
            //feed.AppendChild(author);

            //var id = doc.CreateElement("a", "id", null);
            //id.InnerText = "894090e7-b88e-4e3a-9ff8-eea48848638e";
            //feed.AppendChild(id);

            //var rights = doc.CreateElement("a", "rights", null);
            //rights.InnerText = "Copyright (c) Microsoft Corporation.  All rights reserved.";
            //feed.AppendChild(rights);

            ////foreach ()

            //doc.AppendChild(feed);

            //Stream body = new MemoryStream();
            //doc.Save(body);
            //body.Flush();
            //body.Position = 0;

            //return File(body, "application/xml");
        }

        public async Task<IActionResult> Badges(string member)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Feed
            {
                Id = Guid.Empty.ToString(),
                Links =
                {
                    new Link
                    {
                        Relation = "self",
                        Type = "application/atom+xml",
                        Href = requestUrl
                    }
                },
                Title = new Content
                {
                    Type = "text",
                    Value = member + "'s Badges"
                },
                Entries =
                {
                    new Badge
                    {
                        Description = "Restore the Zune social",
                        TypeId = BadgeType.ActiveForumsBadge_Gold,
                        Title = new Content
                        {
                            Type = "text",
                            Value = "Necromancer"
                        },
                        Image = "https://i.imgur.com/dMwIZs8.png",
                        MediaId = Guid.NewGuid(),
                        MediaType = "Application",
                        Summary = "Where is this shown? No idea, contact YoshiAsk if you see this in the software"
                    }
                }
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", "http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new(typeof(Feed), new[] { typeof(Badge) });

            Console.Out.WriteLine("<!-- " + requestUrl + " -->");
            serializer.Serialize(Console.Out, feed, ns);
            Console.Out.Write("\n\n");

            Stream body = new MemoryStream();
            serializer.Serialize(body, feed, ns);
            body.Flush();
            body.Position = 0;
            return File(body, "application/xml");
        }
    }
}
