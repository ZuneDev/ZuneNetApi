using Atom.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Zune.DB.Models;

namespace Zune.DB.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            ZuneNetContext ctx = new(new ZuneNetContextSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "Zune",
            });

            await ctx.ClearMembersAsync();
            await ctx.ClearTokensAsync();

            string zuneTag = "YoshiAsk";
            var newMember = new Member
            {
                Updated = DateTime.UtcNow,
                Id = Member.GetGuidFromZuneTag(zuneTag),
                UserName = "yoshiask@escargot.chat",
                PlayCount = 206,
                Xuid = Member.GetXuidFromZuneTag(zuneTag),
                ZuneTag = zuneTag,
                DisplayName = "Yoshi Askharoun",
                Status = "Reviving the Zune social",
                Bio = "A computer science student at Texas A&M Univserity that can't help but bring back dead Microsoft products.",
                Location = "College Station, Texas",
                UserTile = "http://tiles.zunes.me/tiles/avatar/default.jpg",
                Background = "http://tiles.zunes.me/tiles/background/USERBACKGROUND-ART-536X196-49.jpg",

                AcceptedTermsOfService = true,
                AccountSuspended = false,
                BillingUnavailable = true,
                SubscriptionLapsed = true,
                TagChangeRequired = false,
                UsageCollectionAllowed = false,
                ExplicitPrivilege = false,
                Lightweight = false,
                Locale = "en-US",
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
            //newMember.Messages ??= new System.Collections.Generic.List<Message>(1);
            //newMember.Messages.Add(message);

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

            await ctx.CreateAsync(newMember);
            //ctx.Messages.Add(message);
            //ctx.Tuners.Add(tuner);

            zuneTag = "WamWooWam";
            var member2 = new Member
            {
                Updated = DateTime.UtcNow,
                Id = Member.GetGuidFromZuneTag(zuneTag),
                PlayCount = 4123,
                Xuid = Member.GetXuidFromZuneTag(zuneTag),
                ZuneTag = "WamWooWam",
                DisplayName = string.Empty,
                Status = "Restoring Windows Phone 7",
                Bio = "he/they, pan, nerd with a strange obsession for windows phone",
                Location = "Ireland",
                UserTile = "http://i.imgur.com/06BuEKG.jpg",
                Background = "http://i.imgur.com/KeZIsxF.jpg",

                AcceptedTermsOfService = true,
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

            await ctx.CreateAsync(member2);
            //ctx.Tuners.Add(tuner);
            //ctx.SaveChanges();
        }
    }
}
