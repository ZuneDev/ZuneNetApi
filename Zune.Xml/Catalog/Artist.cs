using Atom;
using Atom.Attributes;
using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("entry", Namespace = Constants.ATOM_NAMESPACE)]
    [NamespacePrefix("m", Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class Artist : Entry
    {
        [XmlElement("sortTitle", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string SortTitle { get; set; }

        [XmlElement("imageId", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Guid ImageId { get; set; }

        [XmlElement("popularity", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public double Popularity { get; set; }

        [XmlElement("isVariousArtist", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool IsVariousArtist { get; set; }

        [XmlElement("biographyLink", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string BiographyLink { get; set; }

        [XmlElement("biography", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string Biography { get; set; }

        [XmlElement("shortBiography", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string ShortBiography { get; set; }

        [XmlElement("playCount", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public int PlayCount { get; set; }

        [XmlElement("primaryGenre", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Genre PrimaryGenre { get; set; }

        [XmlArray("genres", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("genre", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Genre> Genres { get; set; }

        [XmlArray("moods", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("mood", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Mood> Moods { get; set; }

        [XmlElement("latestAlbumImage", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Image LatestAlbumImage { get; set; }

        [XmlElement("backgroundImage", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Image BackgroundImage { get; set; }

        [XmlElement("hasRadioChannel", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool HasRadioChannel { get; set; }

        [XmlElement("image", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Image> Images { get; set; }
    }
}
