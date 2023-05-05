using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("Backoff")]
    public class Backoff
    {

        [XmlElement("Time")]
        public int Time { get; set; }
    }
}