using Atom;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("entry")]
    public class Track : Media
    {
        [XmlElement("length")]
        public SerializableTimeSpan Duration { get; set; }

        [XmlElement("trackNumber")]
        public int TrackNumber { get; set; }

        [XmlElement("discNumber")]
        public int DiscNumber { get; set; }

        [XmlElement("album")]
        public MiniAlbum Album { get; set; }

        [XmlElement("albumArtist")]
        public MiniArtist AlbumArtist { get; set; }

        [XmlElement("playCount")]
        public int PlayCount { get; set; }

        [XmlElement("referrerContext")]
        public string ReferrerContext { get; set; }

        [XmlElement("musicVideoId")]
        public Guid MusicVideoId { get; set; }


        // Are these valid from the API?

        [XmlElement("PointsPrice")]
        public int PointsPrice { get; set; }

        [XmlElement("CanPlay")]
        public bool CanPlay { get; set; }

        [XmlElement("CanDownload")]
        public bool CanDownload { get; set; }

        [XmlElement("CanPurchase")]
        public bool CanPurchase { get; set; }

        [XmlElement("CanPurchaseMP3")]
        public bool CanPurchaseMP3 { get; set; }

        [XmlElement("CanPurchaseAlbumOnly")]
        public bool CanPurchaseAlbumOnly { get; set; }

        [XmlElement("CanSync")]
        public bool CanSync { get; set; }

        [XmlElement("CanBurn")]
        public bool CanBurn { get; set; }

        [XmlElement("InCollection")]
        public bool InCollection { get; set; }

        [XmlElement("IsDownloading")]
        public bool IsDownloading { get; set; }
    }
}
