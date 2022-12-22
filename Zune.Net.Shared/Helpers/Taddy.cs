using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Zune.Net.Helpers
{
    public static class Taddy
    {
        public const string API_BASE = "https://api.taddy.org";

        public static IFlurlRequest GetBase()
        {
            return API_BASE
                .WithHeader("Content-Type", "application/json")
                .WithHeader("X-USER-ID", Constants.TD_USER_ID)
                .WithHeader("X-API-KEY", Constants.TD_API_KEY);
        }

        public static async Task<TaddyPodcastSeries> GetMinimalPodcastInfo(string name)
        {
            RequestData request = new($"{{ getPodcastSeries(name: \"{name}\") {{ uuid rssUrl description(shouldStripHtmlTags: true) }} }}");

            var response = await GetBase().PostJsonAsync(request);
            var responseObj = await response.GetJsonAsync<JToken>();

            var data = responseObj["data"]["getPodcastSeries"];
            return new(
                Guid.Parse(data.Value<string>("uuid")),
                data.Value<string>("rssUrl"),
                data.Value<string>("description")
            );
        }

        private record RequestData(string query);

        public record TaddyPodcastSeries(Guid Id, string RssUrl, string Description);
    }
}
