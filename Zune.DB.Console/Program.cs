using Atom.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zune.DB.Models;

namespace Zune.DB.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up CLI options
            string connectionString = "mongodb://mongo:27017";
            string dbName = "Zune";
            var options = new OptionSet
            {
                { "h|host=", "The connection string for the MongoDB server.", cs => connectionString = cs },
                { "d|db=", "The database name on the MongoDB server.", db => dbName = db },
            };

            // Parse arguments
            try
            {
                _ = options.Parse(args);
            }
            catch (OptionException e)
            {
                System.Console.Write("ZuneDB: ");
                System.Console.WriteLine(e.Message);
                return;
            }

            // Create DB context
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            ZuneNetContext ctx = new(new ZuneNetContextSettings
            {
                ConnectionString = connectionString,
                DatabaseName = dbName,
            });

            await ctx.ClearMembersAsync();
            await ctx.ClearTokensAsync();
            await ctx.ClearImagesAsync();

            string userName = "yoshiask@escargot.chat";
            var newMember = new Member
            {
                Updated = DateTime.UtcNow,
                Id = Member.GetGuidFromUserName(userName),
                UserName = userName,
                PlayCount = 206,
                Xuid = Member.GetXuidFromUserName(userName),
                ZuneTag = "YoshiAsk",
                DisplayName = "Yoshi Askharoun",
                Status = "Reviving the Zune social",
                Bio = "A computer science student at Texas A&M Univserity that can't help but bring back dead Microsoft products.",
                Location = "College Station, Texas",
                UserTile = "http://tiles.zune.net/tiles/avatar/default.jpg",
                Background = "http://tiles.zune.net/tiles/background/USERBACKGROUND-ART-536X196-49.jpg",

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

            userName = "wamwoowam@escargot.chat";
            var member2 = new Member
            {
                Updated = DateTime.UtcNow,
                Id = Member.GetGuidFromUserName(userName),
                UserName = userName,
                PlayCount = 4123,
                Xuid = Member.GetXuidFromUserName(userName),
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
