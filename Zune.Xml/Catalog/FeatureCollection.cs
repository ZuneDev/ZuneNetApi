using Atom;
using Atom.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class FeatureCollection : Entry
    {
        [XmlElement("index")]
        public int Index { get; set; }

        [XmlElement("link", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public MediaLink Link { get; set; }

        [XmlArray("editorialItems")]
        [XmlArrayItem("editorialItem")]
        public List<Feature> EditorialItems { get; set; }

        [XmlArray("features")]
        [XmlArrayItem("feature")]
        public List<Feature> Features { get; set; }
    }
}
