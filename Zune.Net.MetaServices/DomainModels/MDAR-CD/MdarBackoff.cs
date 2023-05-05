using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    public class MdarBackoff
    {

        [XmlElement("Time")]
        public int Time { get; set; } = 0;
    }
}