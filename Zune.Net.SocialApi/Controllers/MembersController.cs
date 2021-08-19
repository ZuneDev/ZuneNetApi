using System;
using System.Linq;
using Atom;
using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.DB;
using Zune.Xml.SocialApi;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Zune.SocialApi.Controllers
{
    [Route("/members/{zuneTag}/{action=Info}")]
    [Route("/{locale}/members/{zuneTag}/{action=Info}")]
    [Produces("application/atom+xml")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ZuneNetContext _database;
        public MembersController(ZuneNetContext database)
        {
            _database = database;
        }

        public ActionResult<Member> Info(string zuneTag)
        {
            var requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;
            var member = _database.Members.FirstOrDefault(m => m.ZuneTag == zuneTag);

            Member response;
            if (member != null)
            {
                response = member.GetXmlMember();
            }
            else
            {
                return NotFound();
            }

            return response;
        }

        public ActionResult<Feed<Member>> Friends(string zuneTag)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Feed<Member>
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
                    Value = zuneTag + "'s Friends"
                },
                Author = new Author
                {
                    Name = zuneTag,
                    Url = "http://social.zune.net/member/" + zuneTag
                },
                Id = "894090e7-b88e-4e3a-9ff8-eea48848638e",
                Entries = new(),
                Rights = "Copyright (c) Microsoft Corporation.  All rights reserved."
            };

            var member = _database.Members.FirstOrDefault(m => m.ZuneTag == zuneTag);
            if (member == null)
                return NotFound();

            foreach (var relation in member.Friends)
            {
                var friend = relation.MemberB;
                feed.Entries.Add(friend.GetXmlMember());
            }

            return feed;

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

        public ActionResult<Feed<Badge>> Badges(string member)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Feed<Badge>
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

            return feed;
        }
    }
}
