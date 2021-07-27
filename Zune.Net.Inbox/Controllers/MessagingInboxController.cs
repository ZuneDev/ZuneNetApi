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
        public IActionResult List(string locale, string zuneTag)
        {
            string requestUrl = Request.Scheme + "://" + Request.Host + Request.Path;
            Guid msgId = Guid.Parse("7e366cd9-6d16-4ddf-9dfe-963acdef4450");

            var feed = new Feed
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

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", "http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new(typeof(Feed), new[] { typeof(MessageRoot) });

            Console.Out.WriteLine("<!-- " + requestUrl + " -->");
            serializer.Serialize(Console.Out, feed, ns);
            Console.Out.Write("\n\n");

            Stream body = new MemoryStream();
            serializer.Serialize(body, feed, ns);
            body.Flush();
            body.Position = 0;
            return File(body, "application/atom+xml");
        }

        [HttpGet]
        public IActionResult Details(string locale, string zuneTag, string id)
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

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", "http://www.w3.org/2005/Atom");
            XmlSerializer serializer = new(typeof(MessageDetails));

            serializer.Serialize(Console.Out, message, ns);
            Console.Out.Write("\n\n");

            Stream body = new MemoryStream();
            serializer.Serialize(body, message, ns);
            body.Flush();
            body.Position = 0;
            return File(body, "application/atom+xml");
        }
    }
}
