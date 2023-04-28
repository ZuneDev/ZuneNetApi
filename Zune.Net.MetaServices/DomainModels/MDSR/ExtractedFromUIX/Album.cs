using System;

namespace Zune.Xml.MDAR
{
    public class Album{
        public string Title;
        public Int64 Id; //not requred
        public string AlbumArtist;
        public string Genre;
        public int Volume;
        public DateTime ReleaseDate;
        public int NumberOfTracks;
        public Boolean BestMatch;
        public Boolean IsMultiDisk;
        public string CoverParams;
        public string BuyNowParams;
    }
}