using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class AlbumCollection : FeatureCollection
    {
        [XmlArray("albums")]
        [XmlArrayItem("album")]
        public List<Album> Albums { get; set; }
    }
}
