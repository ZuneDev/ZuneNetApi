using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Catalog
{
    [XmlRoot(ElementName = "review", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class Review
    {
        [XmlElement(ElementName = "text")]
         public string Text { get; set; }
         
        [XmlElement(ElementName = "author")]
         public string Author { get; set; }
    }
}