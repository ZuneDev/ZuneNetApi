using Atom.Xml;
using Flurl;
using Flurl.Http;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zune.DataProviders;

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
            var mb_artist = MusicBrainz.Query.LookupArtist(mbid, Include.UrlRelationships | Include.Tags | Include.Releases);
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

        public static async Task<JObject> SearchArtist(string artistName, int? page = null, int? pageSize = null)
        {
            var searchUrl = API_BASE
                .AppendPathSegments("database", "search")
                .SetQueryParam("q", artistName)
                .SetQueryParam("type", "artist")
                .SetQueryParam("page", page)
                .SetQueryParam("per_page", pageSize);

            return await WithAuth(searchUrl).GetJsonAsync<JObject>();
        }

        public static async Task<Content> DCProfileToBiographyContent(string dc_profile)
        {
            // Convert hyperlinks
            Regex rx = new(@"\[([rmal])=?([^\]]+)\]");

            ICollection<Match> matches = rx.Matches(dc_profile);
            StringBuilder htmlBuilder = new();
            int currentIndex = 0;

            foreach (var match in matches)
            {
                var segmentLength = match.Index - currentIndex;
                htmlBuilder.Append(dc_profile, currentIndex, segmentLength);

                currentIndex = match.Index;

                var entityType = match.Groups[1].Value[0];
                var entityName = match.Groups[2].Value;
                var convertedLink = await ConvertDCProfileLinkMarkup(entityType, entityName);
                htmlBuilder.Append(convertedLink);
            }

            string htmlBio = htmlBuilder.ToString();

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

        private static async Task<string> ConvertDCProfileLinkMarkup(char entityType, string arg)
        {
            var linkType = "Unknown";
            var mbid = Guid.Empty;
            string entityName = null;

            if (int.TryParse(arg, out var dcid))
            {
                dcid = -1;
                entityName = arg;
            }

            switch (entityType)
            {
                // Release
                case 'r':
                    linkType = "Album";
                    mbid = MusicBrainz.GetReleaseMBIDByDCID(dcid);

                    if (entityName is null)
                    {
                        var mbRelease = await MusicBrainz.Query.LookupReleaseAsync(mbid);
                        entityName = mbRelease.Title;
                    }
                    break;

                // Artist
                case 'a':
                    linkType = "Contributor";

                    if (dcid < 0)
                    {
                        var searchResponse = await SearchArtist(arg);
                        var searchResults = searchResponse.Value<List<JObject>>("results");
                        var dcArtistResult = searchResults.FirstOrDefault(result =>
                        {
                            var currentName = result.Value<string>("title");
                            return currentName.OrdinalEquals(arg);
                        });

                        dcid = dcArtistResult.Value<int>("id");
                    }

                    mbid = MusicBrainz.GetAristMBIDByDCID(dcid);

                    if (entityName is null)
                    {
                        var mbArtist = await MusicBrainz.Query.LookupArtistAsync(mbid);
                        entityName = mbArtist.Name;
                    }
                    break;

                // Label
                case 'l':
                    linkType = "Label";
                    mbid = MusicBrainz.GetLabelMBIDByDCID(dcid);
                    break;

                // Master
                case 'm':
                    // TODO: Which MusicBrainz entity is this equivalent to? Release groups?
                    break;
            }

            return $"<link type=\"{linkType}\" id=\"{mbid}\">{entityName}</link>";
        }
    }
}
