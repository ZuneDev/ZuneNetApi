using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("category")]
    public class Category
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }
    }

    public class Genre : Category
    {
        public Genre() { }

        public Genre(string id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
