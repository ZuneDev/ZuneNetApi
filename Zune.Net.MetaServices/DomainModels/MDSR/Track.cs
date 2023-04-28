using System;

namespace Zune.Net.MetaServices.DomainModels.MDSR
{
    public class MDARTrack
    {
        public int TrackRequestId {get; set;} = -1; // -1 in example
        public Guid TrackWmid {get; set;} = new Guid();
        public string TrackNum {get; set;} = string.Empty;
        public string Title {get; set;} = string.Empty;
        public string Duration {get; set;} = string.Empty; // mm:ss
        public string UniqueFIleId {get; set;} = string.Empty; //AMGp_id=P 3812;AMGt_id=T 4766848
        public string Performers {get; set;} = string.Empty;
        public string Composers {get; set;} = string.Empty;
        public string Conductors {get; set;} = string.Empty;
        public string Period {get; set;} = string.Empty; //????
        public bool ExplicitLyrics = false;
    }
}