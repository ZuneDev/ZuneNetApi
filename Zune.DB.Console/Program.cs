using Atom.Xml;
using System;
using Zune.DB.Models;
using Zune.Xml;

namespace Zune.DB.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using var ctx = new ZuneNetContext();

            var member = new Member
            {
                Updated = DateTime.UtcNow,
                Id = "894090e7-b88e-4e3a-9ff8-eea48848638e",
                PlayCount = 206,
                ZuneTag = "YoshiAsk",
                DisplayName = string.Empty,
                Status = "Reviving the Zune social",
                Bio = "A computer science student at Texas A&M Univserity that can't help but bring back dead Microsoft products.",
                Location = "College Station, Texas",
                UserTile = "http://web.archive.org/web/20110510042505if_/http://tiles.zune.net/xweb/lx/pic/64x64_tile.jpg",
                Background = "http://cache-tiles.zune.net/tiles/background/YoshiAsk"
            };
            ctx.Members.Add(member);

            Guid msgId = Guid.Parse("7e366cd9-6d16-4ddf-9dfe-963acdef4450");
            var message = new Message
            {
                DetailsLink = string.Empty,
                Sender = member,
                Status = "hi",
                Wishlist = false,
                MediaId = msgId,
                Subject = "Microsoft revives long-dead Zune product line",
                Received = DateTime.UtcNow,
                Type = "message",
                Id = msgId.ToString(),
                AltLink = new Link
                {
                    Href = "https://rr.noordstar.me/microsoft-revives-long-dead-zune-product--f9628d12"
                },
                TextContent = "Tech giant Microsoft announced early Monday morning that a new Zune music player is in the works",
            };
            ctx.Messages.Add(message);

            ctx.SaveChanges();
        }
    }
}
