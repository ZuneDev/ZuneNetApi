using Atom.Xml;
using Flurl;
using Flurl.Http;
using MetaBrainz.Common;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zune.Net.Ontology;

namespace Zune.Net.Helpers
{
    public partial class Discogs
    {
        public static async Task<JObject> GetDCArtistByDCID(ulong dcid)
        {
            return await WithAuth(API_BASE.AppendPathSegments("artists", dcid)).GetJsonAsync<JObject>();
        }

        public static async Task<(JObject dc_artist, IArtist mb_artist)> GetDCArtistByMBID(Guid mbid)
        {
            var mbArtist = await MusicBrainz._query
                .LookupArtistAsync(mbid, Include.UrlRelationships | Include.Tags | Include.Releases);
            
            return (await GetDCArtistByMBArtist(mbArtist), mbArtist);
        }

        public static async Task<JObject> GetDCArtistByMBArtist(IArtist mb_artist)
        {
            var discogs_rel = mb_artist.Relationships.FirstOrDefault(rel => rel.Type == "discogs");
            if (discogs_rel == null)
                return null;
            string discogs_link = discogs_rel.Url.Resource.ToString().Replace("www", "api").Replace("artist", "artists");

            return await WithAuth(discogs_link).GetJsonAsync<JObject>();
        }

        public static Content DCProfileToBiographyContent(string dcProfile)
        {
            // Convert relationships
            Regex rx = new(@"\[([rmal])=?(.+?)\]", RegexOptions.IgnoreCase);
            var htmlBio = rx.Replace(dcProfile, match =>
            {
                var entityType = match.Groups[1].Value[0];
                var htmlEquiv = match.Value;

                var anchorValue = match.Groups[2].Value;
                if (!int.TryParse(anchorValue, out var dcid))
                 return anchorValue;
                
                switch (entityType)
                {
                    // Release
                    case 'r':
                        try
                        {
                            var mbidRel = MusicBrainz.GetReleaseMBIDByDCID(dcid);
                            var mbAlbum = MusicBrainz.GetAlbumByMBID(mbidRel);
                            htmlEquiv = $"<link type=\"Album\" id=\"{mbAlbum.Id}\">{mbAlbum.Title}</link>";
                        }
                        catch (HttpError) { }
                        break;

                    // Artist
                    case 'a':
                        try
                        {
                            var mbidArtist = MusicBrainz.GetAristMBIDByDCID(dcid);
                            var mbArtist = MusicBrainz.GetArtistByMBID(mbidArtist);
                            htmlEquiv = $"<link type=\"Contributor\" id=\"{mbArtist.Id}\">{mbArtist.Title}</link>";
                        }
                        catch (HttpError) { }
                        break;

                    // Label
                    case 'l':
                        try
                        {
                            var mbidLabel = MusicBrainz.GetLabelMBIDByDCID(dcid);
                            // TODO: Label lookup
                            htmlEquiv = $"<link type=\"Label\" id=\"{mbidLabel}\">the label</link>";
                        }
                        catch (HttpError) { }
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
