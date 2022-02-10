using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("imageInstance", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class ImageInstance
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("format")]
        public string Format { get; set; }

        [XmlElement("width")]
        public int Width { get; set; }

        [XmlElement("height")]
        public int Height { get; set; }
    }
}