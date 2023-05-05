using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdarCd
{
    public class MdarCd
    {
        [XmlElement("A_id")]
        public string AId {get; set;}

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
        public Guid AlbumMBID { get; set; }

        [XmlElement("WmCollectiongroupId")]
        public Guid AlbumGroupMBID { get; set; }

        [XmlElement("AlbumWmid")]
        public Guid AlbumWmid { get; set; }

        [XmlElement("ArtistWmid")]
        public Guid ArtistWmid { get; set; }

        [XmlElement("uniqueFileID")]
        public string UniqueFileID { get; set; }

        [XmlElement("Source")]
        public string Source { get; set; } = "AMG";

        [XmlElement("SmallCoverArtURL")]
        public string SmallCoverArtURL { get; set; }

        [XmlElement("LargeCoverArtURL")]
        public string LargeCoverArtURL { get; set; }

        [XmlElement("ReleaseDate")]
        public string DateTime { get; set; }

        [XmlElement("Rating")]
        public string Rating { get; set; } = " ";

        [XmlElement("PerformerName")]
        public string ArtistName { get; set; }

        [XmlElement("MoreInfoLink")]
        public string MoreInfoID { get; set; }

        [XmlElement("Label")]
        public string LabelName { get; set; }

        // Till the response correctly identifies _which_ files are matches, this must be false. Otherwise, you end up with doubled matches
        [XmlElement("IsExactMatch")]
        public bool IsExactMatch { get; set; } = false;

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("DataProviderParams")]
        public string DataProviderParams { get; set; } = "Provider=AMG";

        [XmlElement("DataProviderLogo")]
        public string DataProviderLogo { get; set; } = "Provider=AMG";

        [XmlElement("BuyNowLink")]
        public string BuyNowLink { get; set; }
    }
}