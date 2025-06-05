using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static IRelease GetMBReleaseByDZAlbum(JToken dz_album)
        {
            JToken dz_artist = dz_album["artist"];
            var results = MusicBrainz.Query.FindAllReleases(
                $"artistname:{dz_artist.Value<string>("name")} AND release:{dz_album.Value<string>("title")}", simple: false);

            return results.FirstOrDefault()?.Item;
        }
    }
}
