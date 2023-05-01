using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    public class MdqTrack
    {
        [XmlElement("title")]
        public MdqDescriptionElement Title;

        [XmlElement("artist")]
        public MdqDescriptionElement Artist;

        [XmlElement("trackNumber")]
        public int TrackNumber;

        [XmlElement("trackDuration")]
        public int TrackDurationMs;

        [XmlElement("filename")]
        public string TrackFilename;

        [XmlElement("bitrate")]
        public int Bitrate;

        [XmlElement("drmProtected")]
        public int DRMProtected;

        [XmlElement("trackRequestID")]
        public int trackRequestId;
    }
}