using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("METADATA")]
    public class MdrRequestMetadata
    {

        [XmlElement("MDR-CD")]
        public MdrCd.MDRCD MDRCD { get; set; }

        [XmlElement("Backoff")]
        public Backoff Backoff { get; set; }

        [XmlElement("mdqRequestID")]
        public string MdqRequestID { get; set; }
    }
}
