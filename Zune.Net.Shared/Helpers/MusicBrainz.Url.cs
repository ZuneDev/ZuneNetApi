using MetaBrainz.MusicBrainz;
using System;
using System.Text.RegularExpressions;

namespace Zune.Net.Helpers
{
    public partial class MusicBrainz
    {
        public static readonly Guid UrlIdAllMusic = new Guid("6b3e3c85-0002-4f34-aca6-80ace0d7e846");
        public static readonly Guid UrlIdDiscogs = new Guid("04a5b104-a4c2-4bac-99a1-7b837c37d9e4");
        public static readonly Guid UrlIdLastFm = new Guid("08db8098-c0df-4b78-82c3-c8697b4bba7f");
        public static readonly Guid UrlIdWikidataPerformer = new Guid("689870a4-a1e4-4912-b17f-7b2664215698");
        
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
        
        // URL match patterns from Wikidata wdt:P8966 on each respective Wikidata property 
        // TODO: Is it worth trying to combine similar regexes from each service into one?
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?allmusic\.com\/artist\/(?:[^\/]+)?(mn[0-9]{10})")]
        public static partial Regex RxUrlAllMusicArtist();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?allmusic\.com\/album\/(?:[^\/]+)?(mw[0-9]{10})")]
        public static partial Regex RxUrlAllMusicAlbum();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?allmusic\.com\/album\/release\/(mr\d{10})")]
        public static partial Regex RxUrlAllMusicRelease();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?discogs\.com\/artist\/([1-9][0-9]*)")]
        public static partial Regex RxUrlDiscogsArtist();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?discogs\.com\/(?:[\w\-]+\/)?(?:[^\/]+\/)?release\/([1-9][0-9]*)(?:-[a-zA-Z0-9-]+)")]
        public static partial Regex RxUrlDiscogsRelease();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?discogs\.com\/(?:[a-z]+\/)?(?:[^\/]+\/)?master\/([1-9][0-9]*)(?:-[a-zA-Z0-9-]+)")]
        public static partial Regex RxUrlDiscogsMaster();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?last\.fm\/(?:[a-z]{2}\/)?music\/([^\/\?\#]+)$")]
        public static partial Regex RxUrlLastFm();
    
        [GeneratedRegex(@"^https?:\/\/(?:www\.)?wikidata\.org\/(?:wiki|entity)\/(Q\d+)$")]
        public static partial Regex RxUrlWikidata();
    }
}
