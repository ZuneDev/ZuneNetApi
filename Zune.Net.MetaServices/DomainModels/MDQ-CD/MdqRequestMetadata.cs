using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    [XmlRoot("METADATA")]
    public partial class MdqRequestMetadata
    {
        [XmlElement("MDQ-CD")]
        public MdqCd MdqCd { get; set; } = new();
    }
}