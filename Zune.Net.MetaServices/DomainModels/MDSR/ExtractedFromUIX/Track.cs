using System;

namespace Zune.Xml.MDAR
{
    public class Track
    {
        public string Title;
        public Int64 TrackId;
        public Int64 AlbumId;
        public string Genre;
        public int Volume;
        public DateTime ReleaseDate;
        public int NumberOfTracks;
        public Boolean BestMatch;
        public Boolean IsMultiDisk;
        public int TrackNumber;
        public string CoverParams;
        public string BuyNowParams;
        public string AlbumArtist;
        public string AlbumTitle;
    }
}