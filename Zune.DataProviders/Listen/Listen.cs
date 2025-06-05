using Atom.Xml;
using Newtonsoft.Json.Linq;
using PodcastAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders.Listen
{
    public static partial class Listen
    {
        public static readonly Client _client = new(Constants.LN_API_KEY);

        public static readonly Author LN_AUTHOR = new()
        {
            Name = "ListenNotes",
            Url = "https://www.listennotes.com"
        };

        public static async Task<Feed<PodcastSeries>> GetBestPodcasts(string region = null, int? lnid = null, int limit = int.MaxValue, int page = 1)
        {
            Dictionary<string, string> parameters = new()
            {
                ["page"] = page.ToString(),
            };
            if (lnid != null)
                parameters.Add("genre_id", lnid.ToString());
            if (region != null)
                parameters.Add("region", region.ToLowerInvariant());
            var result = await _client.FetchBestPodcasts(parameters);

            var ln_podcasts = result.ToJSON<JToken>()["podcasts"];
            var updated = DateTime.Now;
            Feed<PodcastSeries> feed = new()
            {
                Id = "podcasts",
                Title = "Podcasts",
                Author = LN_AUTHOR,
                Entries = ln_podcasts.Select(LNPodcastToPodcastSeries).Take(limit).ToList(),
                Updated = updated,
            };

            return feed;
        }

        public static PodcastSeries LNPodcastToPodcastSeries(JToken ln_podcast)
        {
            string authorName = ln_podcast.Value<string>("publisher");

            return new()
            {
                // Convert LNID to GUID just for consistency
                Id = new Guid(ln_podcast.Value<string>("id")).ToString(),
                Title = ln_podcast.Value<string>("title"),
                FeedUrl = ln_podcast.Value<string>("rss"),
                Type = ln_podcast.Value<string>("type"),
                Content = ln_podcast.Value<string>("description"),
                PrimaryArtist = new()
                {
                    Title = authorName,
                },
                Author = new()
                {
                    Name = authorName,
                },
                WebsiteUrl = ln_podcast.Value<string>("website"),
                Explicit = ln_podcast.Value<bool>("explicit_content"),
                ReleaseDate = DateTimeOffset.FromUnixTimeMilliseconds(ln_podcast.Value<long>("earliest_pub_date_ms")).LocalDateTime,
                Updated = DateTimeOffset.FromUnixTimeMilliseconds(ln_podcast.Value<long>("latest_pub_date_ms")).LocalDateTime,
                Images = new()
                {
                    new Image()
                    {
                        Instances = new()
                        {
                            new()
                            {
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
