using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Zune.Net.Helpers
{
    public partial class Deezer
    {
        public static IReleaseGroup GetMBReleaseGroupByDZAlbum(JToken dz_album)
        {
            JToken dz_artist = dz_album["artist"];
            IStreamingQueryResults<MetaBrainz.MusicBrainz.Interfaces.Searches.ISearchResult<IReleaseGroup>>results = MusicBrainz._query.FindAllReleaseGroups(
                $"artistname:{dz_artist.Value<string>("name")} AND releasegroup:{dz_album.Value<string>("title")}", simple: false);

            return results.FirstOrDefault()?.Item;
        }
    }
}
