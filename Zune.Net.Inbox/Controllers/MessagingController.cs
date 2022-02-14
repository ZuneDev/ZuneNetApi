using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.DB;
using Zune.DB.Models;
using Zune.Xml.Inbox;

namespace Zune.Net.Inbox.Controllers
{
    [Route("/{locale}/messaging/{zuneTag}/{action}")]
    [Route("/messaging/{zuneTag}/{action}")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send(string locale, string zuneTag)
        {
            if (!Request.Form.TryGetValue("recipients", out StringValues recipients))
                return StatusCode(StatusCodes.Status400BadRequest, "Message must have at least one recipient.");
            if (!Request.Form.TryGetValue("type", out StringValues type))
                return StatusCode(StatusCodes.Status400BadRequest, "Message must have specify a message type.");

            using var ctx = new ZuneNetContext();
            Member sender = ctx.Members.FirstOrDefault(m => m.ZuneTag == zuneTag);
            if (sender == null)
                return StatusCode(StatusCodes.Status400BadRequest, $"Sender {zuneTag} does not exist.");

            string recipientZuneTag = recipients.First();
            Member recipient;
            if (zuneTag == recipientZuneTag)
                recipient = sender;
            else
                recipient = ctx.Members.FirstOrDefault(m => m.ZuneTag == recipientZuneTag);
            if (recipient == null)
                return StatusCode(StatusCodes.Status400BadRequest, $"Recipient {zuneTag} does not exist.");

            Message msg = new()
            {
                Type = type.Single(),
                Sender = sender,
                Recipient = recipient,
                Id = Guid.NewGuid().ToString(),
                Received = DateTime.UtcNow
            };

            if (Request.Form.TryGetValue("mediaid", out StringValues mediaId))
                msg.MediaId = Guid.Parse(mediaId.Single());
            if (Request.Form.TryGetValue("wishlist", out StringValues wishlist))
                msg.Wishlist = bool.Parse(wishlist);

            ctx.Messages.Add(msg);
            ctx.Members.Attach(sender);
            ctx.Members.Attach(recipient);
            ctx.SaveChanges();
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
