using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    public class MdarTrack
    {
        [XmlElement("Title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement("Performers")]
        public string Performers { get; set; } = string.Empty;

        [XmlElement("TrackNum")]
        public int TrackNumber { get; set; } = 0;

        [XmlElement("TrackWmid")]
        public Guid? TrackWmid {get; set;}

        [XmlElement("TrackRequestID")]
        public int TrackRequestID {get; set;} = 0;
    }
}