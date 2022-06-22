using MetaBrainz.MusicBrainz;
using System;

namespace Zune.Net.Helpers
{
    public partial class MusicBrainz
    {
        public static Guid GetAristMBIDByDCID(int dcid)
        {
            var mb_url = _query.LookupUrl(new Uri($"https://www.discogs.com/artist/{dcid}"), inc: Include.ArtistRelationships);
            return mb_url.Relationships[0].Artist.Id;
        }

        public static Guid GetReleaseMBIDByDCID(int dcid)
        {
            var mb_url = _query.LookupUrl(new Uri($"https://www.discogs.com/release/{dcid}"), inc: Include.ReleaseRelationships);
            return mb_url.Relationships[0].Release.Id;
        }

        public static Guid GetLabelMBIDByDCID(int dcid)
        {
            var mb_url = _query.LookupUrl(new Uri($"https://www.discogs.com/label/{dcid}"), inc:Include.LabelRelationships);
            return mb_url.Relationships[0].Label.Id;
        }
    }
}
