using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("MDR-CD")]
    public class MDRCD
    {
        [XmlElement("mdqRequestID")]
        public Guid MdqRequestID { get; set; } = Guid.Empty;

        [XmlElement("uniqueFileID")]
        public string UniqueFileID { get; set; }  = string.Empty;

        [XmlElement("publisherRating")]
        public string PublisherRating { get; set; } = string.Empty;

        // i.e. providerName=AMG&amp;albumID=DC1A65D9-C8A4-4190-87CB-F871B8AC6D37&amp;a_id=R%20%203021180&amp;album=On%20Empty&amp;artistID=1A9ECD8B-230E-49CD-B97E-1C631530581C&amp;p_id=P%20%202986084&amp;artist=Kevin%20Calder
        [XmlElement("buyParams")]
        public string BuyParams { get; set; } = string.Empty;

        [XmlElement("dataProvider")]
        public string DataProvider { get; set; } = "AMG";

        [XmlElement("dataProviderParams")]
        public string DataProviderParams { get; set; } = "Provider=AMG";

        [XmlElement("dataProviderLogo")]
        public string DataProviderLogo { get; set; } = "Provider=AMG";

        [XmlElement("version")]
        public string Version { get; set; } = "5.0";

        [XmlElement("WMCollectionID")]
        public Guid WMCollectionID { get; set; }

        [XmlElement("WMCollectionGroupID")]
        public Guid WMCollectionGroupID { get; set; }

        [XmlElement("albumTitle")]
        public string AlbumTitle { get; set; } = string.Empty;

        [XmlElement("albumArtist")]
        public string AlbumArtist { get; set; } = string.Empty;

        [XmlElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("label")]
        public string Label { get; set; } = string.Empty;

        [XmlElement("genre")]
        public string Genre { get; set; } = string.Empty;

        // i.e. "Pop/Rock"
        [XmlElement("providerStyle")]
        public string AlbumStyle { get; set; } = string.Empty;

        [XmlElement("needsIDs")]
        public int NeedIDs { get; set; } = 0;

        // i.e. 200/drW500/W564/W56460UCJS0.jpg
        [XmlElement("largeCoverParams")]
        public string LargeCoverAddress { get; set; } = string.Empty;

        // i.e. 075/drW500/W564/W56460UCJS0.jpg
        [XmlElement("smallCoverParams")]
        public string SmallCoverAddress { get; set; } = string.Empty;

        // i.e. a_id=R%20%203021180
        [XmlElement("moreInfoParams")]
        public string MoreInfoId { get; set; } = string.Empty;

        [XmlElement("track")]
        public List<Track> Track { get; set; } = new();

        [XmlElement("Volume")]
        public int VolumeNumber { get; set; }
    }
}