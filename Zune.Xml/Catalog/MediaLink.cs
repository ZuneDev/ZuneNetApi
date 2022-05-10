using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("link", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class MediaLink
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("target")]
        public string Target { get; set; }
    }
}
