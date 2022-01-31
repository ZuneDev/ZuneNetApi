using Atom.Xml;
using System;
using System.Globalization;
using Zune.DB;
using Zune.DB.Models;

namespace Zune.DB.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using var ctx = new ZuneNetContext(true);

            string memberId = "92E7C0FBDC06369A797FDD30AECB92B19089B26D50A9E2384F0C7813E7003A1F";
            Member member = ctx.Members.Find(memberId);
            if (member != null)
            {
                ctx.Members.Remove(member);
                ctx.SaveChanges();
            }

            var newMember = new Member
            {
                Updated = DateTime.UtcNow,
                Id = memberId,
                PlayCount = 206,
                Xuid = memberId,
                ZuneTag = "YoshiAsk",
                DisplayName = string.Empty,
                Status = "Reviving the Zune social",
                Bio = "A computer science student at Texas A&M Univserity that can't help but bring back dead Microsoft products.",
                Location = "College Station, Texas",
                UserTile = "http://web.archive.org/web/20110510042505if_/http://tiles.zune.net/xweb/lx/pic/64x64_tile.jpg",
                Background = "http://cache-tiles.zune.net/tiles/background/YoshiAsk",

                AcceptedTermsOfService = false,
                AccountSuspended = false,
                BillingUnavailable = true,
                SubscriptionLapsed = true,
                TagChangeRequired = false,
                UsageCollectionAllowed = false,
                ExplicitPrivilege = false,
                Lightweight = false,
                Locale = System.Globalization.CultureInfo.CurrentCulture.Name,
                ParentallyControlled = false,
                PointsBalance = 0.0,
                SongCreditBalance = 0.0,
                SongCreditRenewalDate = DateTime.Now.AddDays(1).ToString("O"),
                BillingInstanceId = "6cba2616-c59a-4dd5-bc9e-d41a45215cfa"
            };

            var tuner = new Tuner
            {
                Id = "6cba2616-c59a-4dd5-bc9e-d41a45215cfb"
            };
            newMember.TunerRegisterInfo = tuner;

            Guid msgId = Guid.Parse("7e366cd9-6d16-4ddf-9dfe-963acdef4450");
            Guid linkId = Guid.Parse("fe9ab096-a072-475b-8e24-0aaacf32852d");
            var message = new Message
            {
                DetailsLink = string.Empty,
                Sender = newMember,
                Status = "hi",
                Wishlist = false,
                MediaId = msgId,
                Subject = "Microsoft revives long-dead Zune product line",
                Received = DateTime.UtcNow,
                Type = "message",
                Id = msgId.ToString(),
                AltLink = new Link
                {
                    Href = "https://rr.noordstar.me/microsoft-revives-long-dead-zune-product--f9628d12",
                    Id = linkId.ToString()
                },
                TextContent = "Tech giant Microsoft announced early Monday morning that a new Zune music player is in the works",
            };
            newMember.Messages ??= new System.Collections.Generic.List<Message>(1);
            newMember.Messages.Add(message);

            Guid badgedId = Guid.Parse("fe9ab096-a072-475b-8e24-0aaacf32852f");
            var badge = new Badge
            {
                Description = "Restore the Zune social",
                TypeId = Xml.SocialApi.BadgeType.ActiveForumsBadge_Gold,
                Title = "Necromancer",
                Image = "https://i.imgur.com/dMwIZs8.png",
                MediaId = Guid.NewGuid(),
                MediaType = "Application",
                //Summary = "Where is this shown? No idea, contact YoshiAsk if you see this in the software"
            };

            ctx.Members.Add(newMember);
            ctx.Messages.Add(message);
            ctx.Tuners.Add(tuner);

            memberId = "61E572878F7DBDF918211F0BB7E5715C01CD2B371BA0B4481887939713D48028";
            var member2 = new Member
            {
                Updated = DateTime.UtcNow,
                Id = memberId,
                PlayCount = 4123,
                Xuid = memberId,
                ZuneTag = "WamWooWam",
                DisplayName = string.Empty,
                Status = "Restoring Windows Phone 7",
                Bio = "he/they, pan, nerd with a strange obsession for windows phone",
                Location = "Ireland",
                UserTile = "http://i.imgur.com/06BuEKG.jpg",
                Background = "http://i.imgur.com/KeZIsxF.jpg",

                AcceptedTermsOfService = false,
                AccountSuspended = false,
                BillingUnavailable = true,
                SubscriptionLapsed = true,
                TagChangeRequired = false,
                UsageCollectionAllowed = false,
                ExplicitPrivilege = false,
                Lightweight = false,
                Locale = "en-GB",
                ParentallyControlled = false,
                PointsBalance = 100.0,
                SongCreditBalance = 0.0,
                SongCreditRenewalDate = DateTime.Now.AddDays(1).ToString("O"),
                BillingInstanceId = "6cba2616-c59a-4dd5-bc9e-d41a5f215cfa"
            };

            tuner = new Tuner
            {
                Id = "6cba2616-c59a-4dd5-bc9e-d441f315cfb"
            };
            member2.TunerRegisterInfo = tuner;

            ctx.Members.Add(member2);
            ctx.Tuners.Add(tuner);
            ctx.SaveChanges();
        }
    }
}
