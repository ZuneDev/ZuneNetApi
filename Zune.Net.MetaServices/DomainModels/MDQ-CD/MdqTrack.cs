using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    public class MdqTrack
    {
        [XmlElement("title")]
        public MdqDescriptionElement Title = new();

        [XmlElement("artist")]
        public MdqDescriptionElement Artist = new();

        [XmlElement("trackNumber")]
        public int TrackNumber = 0;

        [XmlElement("trackDuration")]
        public int TrackDurationMs = 0;

        [XmlElement("filename")]
        public string TrackFilename = string.Empty;

        [XmlElement("bitrate")]
        public int Bitrate = 0;

        [XmlElement("drmProtected")]
        public int DRMProtected = 0;

        [XmlElement("trackRequestID")]
        public int trackRequestId = 0;
        [XmlArray("trace")]
        public List<string> Trace = new();
    }
}