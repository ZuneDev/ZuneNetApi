using Atom.Xml;
using Newtonsoft.Json.Linq;
using PodcastAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers
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

        public static async Task<Feed<PodcastSeries>> SearchPodcasts(string query, string region = null, int? pageSize = null, int? offset = null)
        {
            Dictionary<string, string> parameters = new()
            {
                ["q"] = query,
                ["only_in"] = "title,description",
                ["type"] = "podcast",
            };
            if (pageSize is not null)
                parameters.Add("page_size", pageSize.Value.ToString());
            if (offset is not null)
                parameters.Add("offset", offset.Value.ToString());
            if (region != null)
                parameters.Add("region", region.ToLowerInvariant());
            var result = await _client.Search(parameters);

            var ln_podcasts = result.ToJSON<JToken>()["results"];
            var updated = DateTime.Now;
            Feed<PodcastSeries> feed = new()
            {
                Id = "podcasts",
                Title = "Podcasts",
                Author = LN_AUTHOR,
                Entries = ln_podcasts.Select(LNPodcastToPodcastSeries).ToList(),
                Updated = updated,
            };

            return feed;
        }
        
        public static async Task<PodcastSeries> GetPodcast(string id)
        {
            Dictionary<string, string> parameters = new()
            {
                ["id"] = id,
                ["sort"] = "recent_first",
            };
            
            var result = await _client.FetchPodcastById(parameters);

            var ln_podcast = result.ToJSON<JToken>();

            return LNPodcastToPodcastSeries(ln_podcast);
        }

        public static PodcastSeries LNPodcastToPodcastSeries(JToken ln_podcast)
        {
            var authorName = ln_podcast.Value<string>("publisher");
            var description = ln_podcast.Value<string>("description")
                ?? ln_podcast.Value<string>("description_original");
            var title = ln_podcast.Value<string>("title")
                ?? ln_podcast.Value<string>("title_original");

            PodcastSeries podcast = new()
            {
                // Convert LNID to GUID just for consistency
                Id = new Guid(ln_podcast.Value<string>("id")).ToString(),
                Title = title,
                FeedUrl = ln_podcast.Value<string>("rss"),
                Type = ln_podcast.Value<string>("type"),
                Content = new Content
                {
                    Type = ContentType.HTML,
                    Value = description,
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

            return podcast;
        }
    }
}
