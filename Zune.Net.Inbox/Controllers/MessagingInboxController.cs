using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Zune.Xml.Inbox;

namespace Zune.Net.Inbox.Controllers
{
    [ApiController]
    [Route("/{locale}/messaging/{zuneTag}/inbox/{action=List}/{id?}")]
    public class MessagingInboxController : ControllerBase
    {
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
                        DetailsLink = new Link
                        {
                            Href = $"https://inbox.zune.net/{locale}/messaging/{zuneTag}/inbox/details/{msgId}",
                        },
                        From = new Author
                        {
                            Name = "YoshiAsk"
                        },
                        Status = "hi",
                        Wishlist = false,
                        MediaId = msgId,
                        Subject = new Content
                        {
                            Type = "text",
                            Value = "Microsoft revives long-dead Zune product line"
                        },
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
                AltLink = new Link
                {
                    Href = "https://rr.noordstar.me/microsoft-revives-long-dead-zune-product--f9628d12"
                },
                TextContent = "Tech giant Microsoft announced early Monday morning that a new Zune music player is in the works",
                Id = id,
                Updated = DateTime.UtcNow,
                ZuneTag = "YoshiAsk",
                UserTile = "http://web.archive.org/web/20110510042505if_/http://tiles.zune.net/xweb/lx/pic/64x64_tile.jpg"
            };

            return message;
        }
    }
}
