using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using Zune.DB;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/features/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class FeaturesController(ZuneNetContext database) : Controller
    {
        /// <summary>
        /// The URL of the RSS feed to generate features from.
        /// <para>
        /// Currently pulling from NPR's New Music topic.
        /// </para>
        /// </summary>
        private const string RSS_FEED_URL = "https://feeds.npr.org/1108/rss.xml";
        
        private static readonly XNamespace PurlContent = "http://purl.org/rss/1.0/modules/content/";
        
        [HttpGet]
        public async Task<ActionResult<Feed<Feature>>> Features()
        {
            XElement rssDoc = XElement.Load(RSS_FEED_URL);
            var rssChannel = rssDoc.Element("channel");
            Guard.IsNotNull(rssChannel);

            var rssItems = rssChannel.Elements("item");
            List<Feature> features = [];

            //Regex rxParagraph = new("<p>(.+?)</p>");
            Regex rxImg = new("""<img (?<attr>(?<key>[\w-_]+)=['""](?<val>.+?)['""]\s?)+\s*/>""");

            foreach (var rssItem in rssItems)
            {
                Image image = null;
                
                var encodedContent = rssItem.Element(PurlContent + "encoded")?.Value;
                if (encodedContent is not null)
                {
                    var imgTags = rxImg.Matches(encodedContent);
                    foreach (Match imgTag in imgTags)
                    {
                        if (!imgTag.Success)
                            continue;

                        var attrKeys = imgTag.Groups["key"].Captures;
                        var attrValues = imgTag.Groups["val"].Captures;
                        var attributes = attrKeys
                            .Zip(attrValues, (k, v) => (k, v))
                            .ToDictionary(x => x.k.Value, x => x.v.Value);
                        
                        // img tags without an alt attribute probably aren't meant to visible
                        if (!attributes.ContainsKey("alt"))
                            continue;
                        
                        // img tags without a src attribute can't be loaded
                        if (!attributes.TryGetValue("src", out var imgSrc))
                            continue;
                        
                        // Skip images with invalid URLs
                        if (!Uri.TryCreate(imgSrc, UriKind.Absolute, out var imgUri))
                            continue;
                        
                        var imageEntry = await database.AddImageAsync(imgSrc);

                        image = new Image
                        {
                            Id = imageEntry.Id,
                            Instances =
                            [
                                new ImageInstance
                                {
                                    Id = imageEntry.Id,
                                    Url = imageEntry.Url,
                                }
                            ]
                        };
                        break;
                    }
                }
                
                Feature feature = new()
                {
                    Id = rssItem.Element("guid")?.Value,
                    Title = rssItem.Element("title")?.Value,
                    Content = rssItem.Element("description")?.Value,
                    SequenceNumber = features.Count,
                    Image = image,
                };
                
                features.Add(feature);
            }

            Feed<Feature> feed = new()
            {
                Id = "npr-1108",
                Title = rssChannel.Element("title")?.Value ?? "Features",
                Author = new Author
                {
                    Name = "NPR",
                    Url = "https://www.npr.org/",
                },
                Updated = DateTime.TryParse(rssChannel.Element("lastBuildDate")?.Value, out var updated)
                    ? updated : DateTime.Now,
                Entries = features,
            };

            return Ok(feed);
        }
    }
}
