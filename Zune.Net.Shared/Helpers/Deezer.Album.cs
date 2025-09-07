using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static async Task<IRelease> GetMBReleaseByDZAlbumAsync(JToken dz_album)
        {
            JToken dz_artist = dz_album["artist"];
            var results = await MusicBrainz._query.FindReleasesAsync(
                $"artistname:{dz_artist.Value<string>("name")} AND release:{dz_album.Value<string>("title")}",
                limit: 1,
                simple: false);

            return results.Results.FirstOrDefault()?.Item;
        }
    }
}
