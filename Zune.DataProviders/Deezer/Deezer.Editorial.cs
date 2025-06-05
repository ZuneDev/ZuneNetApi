using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static async Task<IEnumerable<JToken>> GetEditorialDZReleases(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("editorial", genreId, "releases")
                .GetJsonAsync<JObject>();
            return response["data"];
        }

        public static async Task<IEnumerable<JToken>> GetEditorialDZSelectAlbums(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("editorial", genreId, "selection")
                .GetJsonAsync<JObject>();
            return response["data"];
        }
    }
}
