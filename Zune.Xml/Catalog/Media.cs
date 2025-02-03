using Atom;
using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    /// <remarks>
    /// <see cref="Entry.Id"/> must be a valid <see cref="Guid"/>.
    /// </remarks>
    public abstract class Media : Entry
    {
        [XmlElement("sortTitle", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string SortTitle { get; set; }

        [XmlArray("rights", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("right", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Right> Rights { get; set; }

        [XmlElement("primaryArtist", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public MiniArtist PrimaryArtist { get; set; }

        [XmlArray("artists", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("artist", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<MiniArtist> Artists { get; set; }

        [XmlElement("primaryGenre", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Genre PrimaryGenre { get; set; }

        [XmlElement("playRank", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public double Popularity { get; set; }

        [XmlElement("isExplicit", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool Explicit { get; set; }

        [XmlElement("isPremium", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool Premium { get; set; }

        [XmlElement("isActionable", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public bool Actionable { get; set; } = true;
    }
}
