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
        [XmlElement("sortTitle")]
        public string SortTitle { get; set; }

        [XmlElement("imageId")]
        public Guid? ImageId { get; set; }

        [XmlArray("rights")]
        [XmlArrayItem("right")]
        public List<Right> Rights { get; set; }

        [XmlElement("primaryArtist")]
        public MiniArtist PrimaryArtist { get; set; }

        [XmlArray("artists")]
        [XmlArrayItem("artist")]
        public List<MiniArtist> Artists { get; set; }

        [XmlElement("primaryGenre")]
        public Genre PrimaryGenre { get; set; }

        [XmlElement("playRank")]
        public double Popularity { get; set; }

        [XmlElement("isExplicit")]
        public bool Explicit { get; set; }

        [XmlElement("isPremium")]
        public bool Premium { get; set; }

        [XmlElement("isActionable")]
        public bool Actionable { get; set; } = true;
    }
}
