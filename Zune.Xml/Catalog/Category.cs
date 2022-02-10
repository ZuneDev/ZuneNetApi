using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public abstract class Category
    {
        [XmlElement("id")]
        public string Id { get; }

        [XmlElement("title")]
        public string Title { get; }
    }

    public class Genre : Category { }
}
