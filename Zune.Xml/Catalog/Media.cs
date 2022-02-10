using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public abstract class Media : Entry
    {
        [XmlElement("id")]
        public new Guid Id { get; set; }

        [XmlElement("sortTitle")]
        public string SortTitle { get; set; }

        [XmlElement("imageId")]
        public Guid ImageId { get; set; }

        [XmlElement("rights")]
        public MediaRights Rights { get; set; }

        [XmlElement("primaryArtist")]
        public MiniArtist PrimaryArtist { get; set; }

        [XmlArray("artists")]
        public IList<MiniArtist> Artists { get; set; }

        [XmlElement("popularity")]
        public double Popularity { get; set; }
    }
}
