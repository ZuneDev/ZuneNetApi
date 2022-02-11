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
        public Guid ImageId { get; set; }

        [XmlElement("rights")]
        public MediaRights Rights { get; set; }

        [XmlElement("primaryArtist")]
        public MiniArtist PrimaryArtist { get; set; }

        [XmlArray("artists")]
        public List<MiniArtist> Artists { get; set; }

        [XmlElement("popularity")]
        public double Popularity { get; set; }
    }
}
