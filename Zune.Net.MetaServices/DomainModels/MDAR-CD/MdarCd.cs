using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    public class MdarCd
    {
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("AlbumId")]
        public long AlbumId { get; set; }

        [XmlElement("Volume")]
        public int Volume { get; set; }

        [XmlElement("track")]
        public List<MdarTrack> Items { get; set; }

        [XmlElement("ReturnCode")]
        public string ReturnCode = "SUCCESS";

        [XmlElement("WmCollectionId")]
        public Guid AlbumMBID {get; set;}

        [XmlElement("WmCollectiongroupId")]
        public Guid AlbumGroupMBID {get; set;}
    }
}