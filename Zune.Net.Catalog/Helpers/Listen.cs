using Atom.Xml;
using Newtonsoft.Json.Linq;
using PodcastAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public static partial class Listen
    {
        public static readonly Client _client = new(Constants.LN_API_KEY);

        public static readonly Author LN_AUTHOR = new()
        {
            Name = "ListenNotes",
            Url = "https://www.listennotes.com"
        };

        public static async Task<Feed<PodcastSeries>> GetBestPodcastsByLNGenre(int? lnid = null, int page = 1)
        {
            Dictionary<string, string> parameters = new()
            {
                ["page"] = page.ToString(),
            };
            if (lnid != null)
                parameters.Add("genre_id", lnid.ToString());
            var result = await _client.FetchBestPodcasts(parameters);

            var ln_podcasts = result.ToJSON<JToken>()["podcasts"];
            var updated = DateTime.Now;
            Feed<PodcastSeries> feed = new()
            {
                Id = "podcasts",
                Title = "Podcasts",
                Author = LN_AUTHOR,
                Entries = ln_podcasts.Select(ln_podcast => LNPodcastToPodcastSeries(ln_podcast)).ToList(),
                Updated = updated,
            };

            return feed;
        }

        public static PodcastSeries LNPodcastToPodcastSeries(JToken ln_podcast)
        {
            return new()
            {
                Id = ln_podcast.Value<string>("id"),
                Title = ln_podcast.Value<string>("title"),
                SourceUrl = ln_podcast.Value<string>("rss"),
                PodcastType = ln_podcast.Value<string>("type"),
                LongDescription = ln_podcast.Value<string>("description"),
                PrimaryArtist = new()
                {
                    Title = ln_podcast.Value<string>("publisher"),
                },
                WebsiteUrl = ln_podcast.Value<string>("website"),
                Explicit = ln_podcast.Value<bool>("explicit_content"),
                EarliestAvailableDate = DateTimeOffset.FromUnixTimeMilliseconds(ln_podcast.Value<long>("earliest_pub_date_ms")).LocalDateTime,
                Images = new()
                {
                    new Image()
                    {
                        Instances = new()
                        {
                            new()
                            {
                                // TODO: Image ID
                                Url = ln_podcast.Value<string>("image"),
                                Format = "jpg",
                            }
                        }
                    }
                }
            };
        }
    }
}
