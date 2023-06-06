using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("METADATA")]
    public class MdrRequestMetadata
    {

        [XmlElement("AlbumId")]
        public Guid? AlbumId { get; set; }
        [XmlElement("MDR-CD")]
        public MDRCD MDRCD { get; set; } = new();

        [XmlElement("Backoff")]
        public Backoff Backoff { get; set; } = new();

        [XmlElement("mdqRequestID")]
        public Guid? MdqRequestID { get; set; }
    }
}
