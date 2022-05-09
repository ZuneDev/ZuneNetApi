using Atom.Xml;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Shared.Helpers.AppleMusic
{
    public partial class AppleMusicClient
    {
        public static async Task<Feed<PodcastSeries>> GetPodcastChart(int limit = 26, int offset = 0, string locale = null)
        {
            locale ??= CultureInfo.CurrentCulture.Name;
            string region = locale.Substring(3, 2);

            var request = GetBase().AppendPathSegments("catalog", region, "charts")
                .SetQueryParam("l", locale).SetQueryParam("types", "podcasts")
                .SetQueryParam("limit", limit)
                .SetQueryParam("offset", offset);
            var response = await request.GetJsonAsync<JToken>();

            var podcastResult = response.Value<JToken>("results").Value<IList<JToken>>("podcasts")[0];
            string nextUrl = AM_HOST_BASE.AppendPathSegment(podcastResult.Value<string>("next"));
            IEnumerable<JToken> amPodcasts = podcastResult.Value<IList<JToken>>("data");

            Feed<PodcastSeries> feed = new()
            {
                Author = AM_AUTHOR,
                Updated = DateTime.Now,
            };

            foreach (var amPodcast in amPodcasts)
            {
                var id = amPodcast.Value<int>("id");
                var attrs = amPodcast.Value<JToken>("attributes");

                var podcast = await LookupPodcast(id);
                podcast.EarliestAvailableDate = DateTime.Parse(attrs.Value<string>("releaseDateTime"));
                podcast.LongDescription = podcast.ShortDescription = attrs.Value<JToken>("description").Value<string>("standard");
                podcast.WebsiteUrl = attrs.Value<string>("websiteUrl");

                feed.Entries.Add(podcast);
            }

            return feed;
        }

        public static async Task<PodcastSeries> LookupPodcast(int id)
        {
            var request = IT_HOST_BASE.AppendPathSegment("lookup")
                .SetQueryParam("id", id);
            var response = await request.GetJsonAsync<JToken>();

            var amPodcast = response.Value<IList<JToken>>("results")[0];

            MiniArtist artist = new()
            {
                Title = amPodcast.Value<string>("artistName")
            };

            PodcastSeries podcast = new()
            {
                Id = new Guid(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0).ToString(),
                Title = amPodcast.Value<string>("trackName"),
                SourceUrl = amPodcast.Value<string>("feedUrl"),
                Premium = amPodcast.Value<double>("trackPrice") > 0,
                Artists = new() { artist },
                PrimaryArtist = artist,
                PrimaryGenre = new()
                {
                    Id = amPodcast.Value<IList<JToken>>("genreIds")[0].Value<string>(),
                    Title = amPodcast.Value<IList<JToken>>("genres")[0].Value<string>()
                },
                Images = new()
                {
                    new Image()
                    {
                        Instances = new()
                        {
                            new ImageInstance
                            {
                                Width = 30,
                                Height = 30,
                                Url = amPodcast.Value<string>("artworkUrl30")
                            },
                            new ImageInstance
                            {
                                Width = 60,
                                Height = 60,
                                Url = amPodcast.Value<string>("artworkUrl60")
                            },
                            new ImageInstance
                            {
                                Width = 100,
                                Height = 100,
                                Url = amPodcast.Value<string>("artworkUrl100")
                            },
                            new ImageInstance
                            {
                                Width = 600,
                                Height = 600,
                                Url = amPodcast.Value<string>("artworkUrl600")
                            },
                        }
                    }
                }
            };

            return podcast;
        }
    }
}
