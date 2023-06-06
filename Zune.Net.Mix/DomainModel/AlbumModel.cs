using System.Xml.Serialization;
namespace Zune.Net.Mix.DomainModel
{
    [XmlRoot(ElementName = "feed")]
    public class AlbumModel
    {
        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; } = DateTime.Now;

        [XmlElement(ElementName = "entry")]
        public List<VectorEntry> Entry { get; set; } = new();
    }
}
