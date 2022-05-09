using Flurl.Http;

namespace Zune.Net.Shared.Helpers
{
    public static partial class Discogs
    {
        public const string API_BASE = "https://api.discogs.com";

        public static IFlurlRequest WithAuth(string url)
        {
            return url.WithHeader("User-Agent", Constants.USERAGENT_ZUNE48_GITHUB)
                      .WithHeader("Authorization", $"Discogs key={Constants.DC_API_KEY}, secret={Constants.DC_API_SECRET}");
        }
    }
}
