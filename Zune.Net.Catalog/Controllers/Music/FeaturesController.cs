using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleMusicSharp;
using Zune.DB;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/features/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class FeaturesController(ZuneNetContext database, AppleMusicClient amClient) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<Feed<Feature>>> Features()
        {
            var grouping = await amClient.GetGroupingAsync("1");
            var editorials = grouping.Resources.EditorialElements.Values.AsEnumerable();
            
            List<Feature> features = [];
            var lastUpdated = DateTime.MinValue;

            foreach (var editorial in editorials)
            {
                Image image = null;
                MediaLink link = null;

                var subscriptionCover = editorial.Attributes.EditorialArtwork?.SubscriptionCover;
                if (subscriptionCover is not null)
                {
                    var imageUrl = AppleMusicClient.GetImageUrl(subscriptionCover.Url,
                        420, 320, resizeMode: ResizeMode.ScaleCrop);
                        
                    var imageEntry = await database.AddImageAsync(imageUrl);

                    image = new Image
                    {
                        Id = imageEntry.Id,
                        Instances =
                        [
                            new ImageInstance
                            {
                                Id = imageEntry.Id,
                                Url = imageEntry.Url
                            }
                        ]
                    };
                }

                var relationships = editorial.Relationships?.Contents?.Data?.FirstOrDefault();
                if (relationships is null)
                    continue;

                if (grouping.Resources.MusicVideos.TryGetValue(relationships.Id, out var video))
                {
                    link = new MediaLink
                    {
                        Type = relationships.Type,
                        Target = video.Attributes.Url
                    };
                }

                Feature feature = new()
                {
                    Id = editorial.Id,
                    Title = editorial.Attributes.DesignBadge,
                    Content = editorial.Attributes.DesignTag,
                    SequenceNumber = features.Count,
                    Link = link,
                    Image = image,
                };
                
                features.Add(feature);
                
                var editorialLastModified = editorial.Attributes.LastModifiedDate.DateTime;
                if (editorialLastModified > lastUpdated)
                    lastUpdated = editorialLastModified;
            }

            Feed<Feature> feed = new()
            {
                Id = grouping.Data.First().Href,
                Title = "Features",
                Author = new Author
                {
                    Name = "Apple Music",
                    Url = "https://music.apple.com/",
                },
                Updated = lastUpdated,
                Entries = features,
            };

            return Ok(feed);
        }
    }
}
