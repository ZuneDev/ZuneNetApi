using Atom;
using System;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class Track : Media
    {
        [XmlElement("length", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public SerializableTimeSpan Duration { get; set; }

        [XmlElement("trackNumber", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public int TrackNumber { get; set; }

        [XmlElement("discNumber", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public int DiscNumber { get; set; }

        [XmlElement("album", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public MiniAlbum Album { get; set; }

        [XmlElement("albumArtist", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public MiniArtist AlbumArtist { get; set; }

        [XmlElement("playCount", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public int PlayCount { get; set; }

        [XmlElement("referrerContext", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string ReferrerContext { get; set; }

        [XmlElement("musicVideoId", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Guid MusicVideoId { get; set; }


        // Are these valid from the API?

        [XmlElement("PointsPrice", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public int PointsPrice { get; set; }

        [XmlElement("CanPlay", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanPlay { get; set; }

        [XmlElement("CanDownload", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanDownload { get; set; }

        [XmlElement("CanPurchase", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanPurchase { get; set; }

        [XmlElement("CanPurchaseMP3", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanPurchaseMP3 { get; set; }

        [XmlElement("CanPurchaseAlbumOnly", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanPurchaseAlbumOnly { get; set; }

        [XmlElement("CanSync", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanSync { get; set; }

        [XmlElement("CanBurn", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool CanBurn { get; set; }

        [XmlElement("InCollection", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool InCollection { get; set; }

        [XmlElement("IsDownloading", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool IsDownloading { get; set; }
    }
}
