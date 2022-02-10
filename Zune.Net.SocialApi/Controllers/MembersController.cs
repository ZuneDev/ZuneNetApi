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
                Links = { new Link(requestUrl) },
                Updated = DateTimeOffset.UtcNow,
                Title = zuneTag + "'s Friends",
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
        }

        public ActionResult<Feed<Badge>> Badges(string member)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var feed = new Feed<Badge>
            {
                Id = Guid.Empty.ToString(),
                Links = { new Link(requestUrl) },
                Title = member + "'s Badges",
                Entries =
                {
                    new Badge
                    {
                        Description = "Restore the Zune social",
                        TypeId = BadgeType.ActiveForumsBadge_Gold,
                        Title = "Necromancer",
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
