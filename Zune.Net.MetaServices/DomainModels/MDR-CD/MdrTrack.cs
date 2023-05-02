using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("track")]
    public class Track
    {

        [XmlElement("uniqueFileID")]
        public object UniqueFileID { get; set; }

        [XmlElement("WMContentID")]
        public string WMContentID { get; set; }

        [XmlElement("trackRequestID")]
        public int TrackRequestID { get; set; }

        [XmlElement("trackTitle")]
        public string TrackTitle { get; set; }

        [XmlElement("trackNumber")]
        public int TrackNumber { get; set; }

        [XmlElement("trackPerformer")]
        public int TrackPerformer { get; set; }

        [XmlElement("trackComposer")]
        public object TrackComposer { get; set; }

        [XmlElement("trackConductor")]
        public object TrackConductor { get; set; }

        [XmlElement("period")]
        public string Period { get; set; }

        [XmlElement("explicitLyrics")]
        public int ExplicitLyrics { get; set; }
    }
}