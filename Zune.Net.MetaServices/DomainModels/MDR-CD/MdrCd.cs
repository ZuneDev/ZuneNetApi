using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdrCd
{
    [XmlRoot("MDR-CD")]
    public class MDRCD
    {

        [XmlElement("uniqueFileID")]
        public object UniqueFileID { get; set; }

        [XmlElement("providerStyle")]
        public object ProviderStyle { get; set; }

        [XmlElement("publisherRating")]
        public object PublisherRating { get; set; }

        [XmlElement("buyParams")]
        public object BuyParams { get; set; }

        [XmlElement("moreInfoParams")]
        public object MoreInfoParams { get; set; }

        [XmlElement("dataProvider")]
        public object DataProvider { get; set; }

        [XmlElement("dataProviderParams")]
        public object DataProviderParams { get; set; }

        [XmlElement("dataProviderLogo")]
        public object DataProviderLogo { get; set; }

        [XmlElement("version")]
        public int Version { get; set; }

        [XmlElement("mdqRequestID")]
        public string MdqRequestID { get; set; }

        [XmlElement("WMCollectionID")]
        public string WMCollectionID { get; set; }

        [XmlElement("WMCollectionGroupID")]
        public string WMCollectionGroupID { get; set; }

        [XmlElement("albumTitle")]
        public string AlbumTitle { get; set; }

        [XmlElement("albumArtist")]
        public int AlbumArtist { get; set; }

        [XmlElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("label")]
        public string Label { get; set; }

        [XmlElement("genre")]
        public string Genre { get; set; }

        [XmlElement("needIDs")]
        public int NeedIDs { get; set; }

        [XmlElement("track")]
        public List<Track> Track { get; set; }
    }
}