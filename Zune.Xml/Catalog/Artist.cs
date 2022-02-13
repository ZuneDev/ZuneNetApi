using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("entry")]
    public class Artist : Entry
    {
        [XmlElement("sortTitle")]
        public string SortTitle { get; set; }

        [XmlElement("imageId")]
        public Guid ImageId { get; set; }

        [XmlElement("popularity")]
        public double Popularity { get; set; }

        [XmlElement("isVariousArtist")]
        public bool IsVariousArtist { get; set; }

        [XmlElement("biographyLink")]
        public string BiographyLink { get; set; }

        [XmlElement("biography")]
        public string Biography { get; set; }

        [XmlElement("shortBiography")]
        public string ShortBiography { get; set; }

        [XmlElement("playCount")]
        public int PlayCount { get; set; }

        [XmlElement("primaryGenre")]
        public Genre PrimaryGenre { get; set; }

        [XmlArray("genres")]
        [XmlArrayItem("genre")]
        public List<Genre> Genres { get; set; }

        [XmlArray("moods")]
        [XmlArrayItem("mood")]
        public List<Mood> Moods { get; set; }

        [XmlElement("albumImageId")]
        public Guid AlbumImageId { get; set; }

        [XmlElement("backgroundImageId")]
        public Guid BackgroundImageId { get; set; }

        [XmlElement("hasRadioChannel")]
        public bool HasRadioChannel { get; set; }

        [XmlElement("image")]
        public List<Image> Images { get; set; }
    }
}
