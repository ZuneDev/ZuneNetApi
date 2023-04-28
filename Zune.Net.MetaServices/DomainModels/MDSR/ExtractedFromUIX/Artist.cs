using System;

namespace Zune.Xml.MDAR
{
    // Artist List should return a list of these
    public class Artist
    {
        public string ArtistName;
        public Int64 Id;
        public string ImageParams;
        public int AlbumCount;
        public Boolean BestMatch;
        public int TrackCount;
        public string Genre;
    }
}