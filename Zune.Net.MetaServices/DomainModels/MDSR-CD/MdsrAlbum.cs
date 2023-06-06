using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdsrCd
{
    public class MdsrAlbum
    {
        [XmlElement("albumFullTitle")]
        public string Title { get; set; } = string.Empty;

        [XmlElement("id_album")]
        public long AlbumId { get; set; }

        [XmlElement("albumPerformer")]
        public string AlbumArtist { get; set; } = string.Empty;

        [XmlElement("albumGenre")]
        public string Genre { get; set; } = string.Empty;

        [XmlElement("Volume")]
        public int Volume { get; set; }

        [XmlElement("albumReleaseDate")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("numberOfTracks")]
        public int NumberOfTracks { get; set; }

        [XmlElement("bestmatch")]
        public bool BestMatch { get; set; }

        [XmlElement("IsMultiDisc")]
        public bool IsMultiDisc { get; set; }

        [XmlElement("albumCover")]
        public string CoverParms { get; set; } = string.Empty;

        [XmlElement("buyNowLink")]
        public string BuyNowParms { get; set; } = string.Empty;
    }
}