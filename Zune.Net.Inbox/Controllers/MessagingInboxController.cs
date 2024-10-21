using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.DB;
using Zune.DB.Models;
using Zune.Xml.Inbox;

namespace Zune.Net.Inbox.Controllers
{
    [ApiController]
    [Route("/{locale}/messaging/{zuneTag}/inbox/{action=List}/{id?}")]
    [Route("/messaging/{zuneTag}/inbox/{action=List}/{id?}")]
    public class MessagingInboxController : ControllerBase
    {
        private readonly ZuneNetContext _context;

        public MessagingInboxController(ZuneNetContext context)
        {
            _context = context;
        }

        [HttpGet]
        public Feed<MessageRoot> List(string locale, string zuneTag)
        {
            var requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;
            var msgId = Guid.Parse("7e366cd9-6d16-4ddf-9dfe-963acdef4450");

            var feed = new Feed<MessageRoot>
            {
                Entries =
                {
                    new MessageRoot
                    {
                        DetailsLink = new($"https://inbox.zune.net/{locale}/messaging/{zuneTag}/inbox/details/{msgId}"),
                        From = new Author
                        {
                            Name = "YoshiAsk"
                        },
                        Status = "hi",
                        Wishlist = false,
                        MediaId = msgId,
                        Subject = "Microsoft revives long-dead Zune product line",
                        Received = DateTime.UtcNow,
                        Type = "message"
                    }
                }
            };

            return feed;
        }

        [HttpGet]
        public ActionResult<MessageDetails> Details(string locale, string zuneTag, string id)
        {
            var message = new MessageDetails
            {
                AltLink = new("https://rr.noordstar.me/microsoft-revives-long-dead-zune-product--f9628d12"),
                TextContent = "Tech giant Microsoft announced early Monday morning that a new Zune music player is in the works",
                Id = id,
                Updated = DateTime.UtcNow,
                ZuneTag = "YoshiAsk",
                UserTile = "http://web.archive.org/web/20110510042505if_/http://tiles.zune.net/xweb/lx/pic/64x64_tile.jpg"
            };

            return message;
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCont(string locale, string zuneTag)
        {
            Member member = await _context.GetSingleAsync(m => m.ZuneTag == zuneTag);
            if (member == null)
                return BadRequest($"User {zuneTag} does not exist.");

            return Content(member.Messages.Count(msg => !msg.IsRead).ToString());
        }
    }
}
