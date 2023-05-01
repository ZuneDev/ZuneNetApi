using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    public class MdqCd
    {
        [XmlElement("mdqRequestID")]
        public string MdqRequestId { get; set; }

        [XmlElement("album")]
        public MdqAlbum Album { get; set; }

        [XmlElement("track")]
        public List<MdqTrack> Tracks { get; set; }
    }
}