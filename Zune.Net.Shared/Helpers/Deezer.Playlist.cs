using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static IRelease GetMBReleaseByDZPlaylist(JToken dz_album)
        {
            JToken dz_user = dz_album["user"];
            var results = MusicBrainz._query.FindAllReleases(
                $"artistname:{dz_user.Value<string>("name")} AND release:{dz_album.Value<string>("title")}", simple: false);

            return results.FirstOrDefault()?.Item;
        }
    }
}
