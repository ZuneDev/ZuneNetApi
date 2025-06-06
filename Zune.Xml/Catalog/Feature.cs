using Atom;
using Atom.Xml;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class Feature
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("link", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public MediaLink Link { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("content")]
        public Content Content { get; set; }

        [XmlElement("image")]
        public Image Image { get; set; }

        [XmlElement("isExplicit")]
        public bool Explicit { get; set; }
    }
}