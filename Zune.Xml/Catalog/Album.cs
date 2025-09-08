using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("feed")]
    public class Album : Media
    {
        [XmlElement("image")]
        public List<Image> Images { get; set; }

        [XmlElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement(ElementName = "entry")]
        public List<Track> Tracks { get; set; }
        
        [XmlElement(ElementName = "review")]
        public Review Review { get; set; }
    }
}
