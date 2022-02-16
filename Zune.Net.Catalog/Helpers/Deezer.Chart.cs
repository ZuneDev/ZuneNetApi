using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.Net.Catalog.Helpers
{
    public partial class Deezer
    {
        public static async Task<IEnumerable<JToken>> GetChartDZTracks(int genreId = 0)
        {
            var response = await API_BASE.AppendPathSegments("chart", genreId, "tracks")
                .GetJsonAsync<JObject>();
            var dz_tracks = response["data"];
            return dz_tracks;
        }
    }
}
