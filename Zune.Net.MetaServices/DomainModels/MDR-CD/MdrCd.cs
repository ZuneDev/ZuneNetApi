using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("MDR-CD")]
    public class MDRCD
    {

        [XmlElement("uniqueFileID")]
        public string UniqueFileID { get; set; }

        [XmlElement("providerStyle")]
        public string ProviderStyle { get; set; }

        [XmlElement("publisherRating")]
        public string PublisherRating { get; set; }

        [XmlElement("buyParams")]
        public string BuyParams { get; set; }

        [XmlElement("moreInfoParams")]
        public string MoreInfoParams { get; set; }

        [XmlElement("dataProvider")]
        public string DataProvider { get; set; }

        [XmlElement("dataProviderParams")]
        public string DataProviderParams { get; set; }

        [XmlElement("dataProviderLogo")]
        public string DataProviderLogo { get; set; }

        [XmlElement("version")]
        public int Version { get; set; }

        [XmlElement("mdqRequestID")]
        public Guid MdqRequestID { get; set; }

        [XmlElement("WMCollectionID")]
        public Guid WMCollectionID { get; set; }

        [XmlElement("WMCollectionGroupID")]
        public Guid WMCollectionGroupID { get; set; }

        [XmlElement("albumTitle")]
        public string AlbumTitle { get; set; }

        [XmlElement("albumArtist")]
        public string AlbumArtist { get; set; }

        [XmlElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("label")]
        public string Label { get; set; }

        [XmlElement("genre")]
        public string Genre { get; set; }

        [XmlElement("needIDs")]
        public int NeedIDs { get; set; }

        [XmlElement("largeCoverParams")]
        public string LargeCoverAddress { get; set; }

        [XmlElement("smallCoverParams")]
        public string SmallCoverAddress { get; set; }

        [XmlElement("track")]
        public List<Track> Track { get; set; }
    }
}