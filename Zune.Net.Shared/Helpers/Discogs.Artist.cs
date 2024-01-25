using Atom.Xml;
using Flurl;
using Flurl.Http;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zune.Net.Helpers
{
    public partial class Discogs
    {
        public static async Task<JObject> GetDCArtistByDCID(int dcid)
        {
            return await WithAuth(API_BASE.AppendPathSegments("artists", dcid)).GetJsonAsync<JObject>();
        }

        public static async Task<(JObject dc_artist, IArtist mb_artist)> GetDCArtistByMBID(Guid mbid)
        {
            var mb_artist = MusicBrainz._query.LookupArtist(mbid, Include.UrlRelationships | Include.Tags | Include.Releases);
            return (await GetDCArtistByMBArtist(mb_artist), mb_artist);
        }

        public static async Task<JObject> GetDCArtistByMBArtist(IArtist mb_artist)
        {
            var discogs_rel = mb_artist.Relationships.FirstOrDefault(rel => rel.Type == "discogs");
            if (discogs_rel == null)
                return null;
            string discogs_link = discogs_rel.Url.Resource.ToString().Replace("www", "api").Replace("artist", "artists");

            return await WithAuth(discogs_link).GetJsonAsync<JObject>();
        }

        public static Content DCProfileToBiographyContent(string dc_profile)
        {
            // Convert relationships
            Regex rx = new(@"\[([rmal])=?(\d+)\]", RegexOptions.IgnoreCase);
            string htmlBio = rx.Replace(dc_profile, match =>
            {
                var entityType = match.Groups[1].Value[0];
                int dcid = int.Parse(match.Groups[2].Value);
                string htmlEquiv = match.Value;
                switch (entityType)
                {
                    // Release
                    case 'r':
                        try
                        {
                            var mbid_rel = MusicBrainz.GetReleaseMBIDByDCID(dcid);
                            var mb_album = MusicBrainz.GetAlbumByMBID(mbid_rel);
                            htmlEquiv = $"<link type=\"Album\" id=\"{mb_album.Id}\">{mb_album.Title}</link>";
                        }
                        catch (Exception e) { Console.WriteLine(e);}
                        break;

                    // Artist
                    case 'a':
                        try
                        {
                            var mbid_artist = MusicBrainz.GetAristMBIDByDCID(dcid);
                            var mb_artist = MusicBrainz.GetArtistByMBID(mbid_artist);
                            htmlEquiv = $"<link type=\"Contributor\" id=\"{mb_artist.Id}\">{mb_artist.Title}</link>";
                        }
                        catch (Exception e) { Console.WriteLine(e);}
                        break;

                    // Label
                    case 'l':
                        try
                        {
                            var mbid_label = MusicBrainz.GetLabelMBIDByDCID(dcid);
                            // TODO: Label lookup
                            htmlEquiv = $"<link type=\"Label\" id=\"{mbid_label}\">the label</link>";
                        }
                        catch (Exception e) { Console.WriteLine(e);}
                        break;

                    // Master
                    case 'm':
                        // TODO: Which MusicBrainz entity is this equivalent to? Release groups?
                        break;
                }

                return htmlEquiv;
            });

            // Convert formatting
            rx = new(@"\[([bi])\](.*)\[\/\1\]", RegexOptions.IgnoreCase);
            htmlBio = rx.Replace(htmlBio, @"<$1>$2</$1>");

            // Convert links
            rx = new(@"\[url=([^\]]*)\]([^\[]*)?\[\/url\]", RegexOptions.IgnoreCase);
            htmlBio = rx.Replace(htmlBio, "<link href=\"$1\">$2</link>");

            return new()
            {
                Type = ContentType.HTML,
                Value = htmlBio
            };
        }
    }
}
