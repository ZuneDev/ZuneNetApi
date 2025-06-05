using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static async Task<IEnumerable<JToken>> GetChartDZTracks(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "tracks")
                .GetJsonAsync<JObject>();
            return response["data"];
        }

        public static async Task<IEnumerable<JToken>> GetChartDZAlbums(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "albums")
                .GetJsonAsync<JObject>();
            return response["data"];
        }

        public static async Task<IEnumerable<JToken>> GetChartDZArtists(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "albums")
                .GetJsonAsync<JObject>();
            return response["data"];
        }

        public static async Task<IEnumerable<JToken>> GetChartDZPlaylists(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "playlists")
                .GetJsonAsync<JObject>();
            return response["data"];
        }

        public static async Task<IEnumerable<PodcastSeries>> GetChartPodcasts(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "podcasts")
                .GetJsonAsync<JObject>();
            return response["data"].Select(dz_podcast => DZPodcastToPodcastSeries(dz_podcast));
        }
    }
}
