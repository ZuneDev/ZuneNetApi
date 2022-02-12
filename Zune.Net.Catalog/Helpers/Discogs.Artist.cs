using Flurl.Http;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.Net.Catalog.Helpers
{
    public partial class Discogs
    {
        public static async Task<(JObject dc_artist, IArtist mb_artist)> GetDCArtistByMBID(Guid mbid)
        {
            var mb_artist = MusicBrainz._query.LookupArtist(mbid, Include.UrlRelationships | Include.Tags);
            return (await GetDCArtistByMBArtist(mb_artist), mb_artist);
        }

        public static async Task<JObject> GetDCArtistByMBArtist(IArtist mb_artist)
        {
            var discogs_rel = mb_artist.Relationships.First(rel => rel.Type == "discogs");
            string discogs_link = discogs_rel.Url.Resource.ToString().Replace("www", "api").Replace("artist", "artists");

            return await WithAuth(discogs_link).GetJsonAsync<JObject>();
        }
    }
}
