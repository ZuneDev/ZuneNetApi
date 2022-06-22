﻿using System;
using System.Linq;
using System.Threading.Tasks;
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
        
        public async Task<ActionResult<Member>> Info(string zuneTag)
        {
            var member = await _database.GetByIdOrZuneTag(zuneTag);

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

        public async Task<ActionResult<Feed<Member>>> Friends(string zuneTag)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var member = await _database.GetByIdOrZuneTag(zuneTag);
            if (member == null)
                return NotFound();

            var feed = new Feed<Member>
            {
                Namespace = Constants.ZUNE_PROFILES_NAMESPACE,
                Links = { new Link(requestUrl) },
                Updated = DateTime.UtcNow,
                Title = member.ZuneTag + "'s Friends",
                Author = new Author
                {
                    Name = member.ZuneTag,
                    Url = "http://social.zune.net/member/" + member.ZuneTag
                },
                Id = member.Id.ToString(),
                Entries = new(),
                Rights = "Copyright (c) Microsoft Corporation.  All rights reserved."
            };

            if (member.Friends != null)
                foreach (var friendId in member.Friends)
                {
                    var friend = await _database.GetAsync(friendId);
                    feed.Entries.Add(friend.GetXmlMember());
                }

            return feed;
        }

        public async Task<ActionResult<Feed<Badge>>> Badges(string zuneTag)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;

            var member = await _database.GetByIdOrZuneTag(zuneTag);
            if (member == null)
                return NotFound();

            Badge badge1 = new()
            {
                Description = "Restore the Zune social",
                TypeId = BadgeType.ActiveForumsBadge_Gold,
                Title = "Necromancer",
                Image = "https://i.imgur.com/dMwIZs8.png",
                Media = new()
                {
                    Id = Guid.NewGuid(),
                    Type = "Application",
                }
            };

            var feed = new Feed<Badge>
            {
                Id = Guid.Empty.ToString(),
                Links = { new Link(requestUrl) },
                Title = member.ZuneTag + "'s Badges",
                Entries =
                {
                    badge1
                }
            };

            return feed;
        }
    }
}
