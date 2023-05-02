using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    public class MdarTrack
    {
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Performers")]
        public string Performers { get; set; }

        [XmlElement("TrackNum")]
        public int TrackNumber { get; set; }
    }
}