using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("track")]
    public class Track
    {

        [XmlElement("uniqueFileID")]
        public string UniqueFileID { get; set; } = string.Empty;

        [XmlElement("WMContentID")]
        public string WMContentID { get; set; } = string.Empty;

        [XmlElement("trackRequestID")]
        public int TrackRequestID { get; set; }

        [XmlElement("trackTitle")]
        public string TrackTitle { get; set; } = string.Empty;

        [XmlElement("trackNumber")]
        public string TrackNumber { get; set; } = string.Empty;

        [XmlElement("trackPerformer")]
        public string TrackPerformer { get; set; } = string.Empty;

        [XmlElement("trackComposer")]
        public string TrackComposer { get; set; } = string.Empty;

        [XmlElement("trackConductor")]
        public string TrackConductor { get; set; } = string.Empty;

        [XmlElement("period")]
        public string Period { get; set; } = string.Empty;

        [XmlElement("explicitLyrics")]
        public int ExplicitLyrics { get; set; } = 0;
    }
}