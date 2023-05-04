using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Zune.DB.Models
{
    public class WMISAlbumTrackEntry
    {
        [BsonId]
        public string RecordId { get; set; }
        public Guid AlbumMbid { get; set; } // musicbrainz id, so we can fetch the thing later
        public Guid TrackMbid { get; set; }
        public int TrackId { get; set; } // this is the ID that is used by the WMIS document
        public int TrackDuration { get; set; }

        public WMISAlbumTrackEntry(int trackId, int trackDuration, Guid albumMbid, Guid trackMbid)
        {
            RecordId = (trackId + trackDuration).ToString();
            TrackId = trackId;
            TrackDuration = trackDuration;
            AlbumMbid = albumMbid;
            TrackMbid = trackMbid;
        }
    }
}