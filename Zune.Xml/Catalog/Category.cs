using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public abstract class Category
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }
    }

    public class Genre : Category { }
}
