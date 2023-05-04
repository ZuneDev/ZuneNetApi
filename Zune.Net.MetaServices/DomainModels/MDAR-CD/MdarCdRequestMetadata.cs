using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    [XmlRoot("METADATA")]
    public class MdarCdRequestMetadata
    {
        [XmlElement("mdqRequestID")]
        public Guid mdqRequestID{get; set;}

        [XmlElement("MDAR-CD")]
        public MdarCd MdarCd {get; set;}
    }
}