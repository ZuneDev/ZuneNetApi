using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("mood")]
    public class Mood
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }
    }
}