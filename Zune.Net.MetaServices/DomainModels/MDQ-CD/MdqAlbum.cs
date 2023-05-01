using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    public class MdqAlbum
    {
        [XmlElement("title")]
        public MdqDescriptionElement AlbumTitle;

        [XmlElement("artist")]
        public MdqDescriptionElement AlbumArtist;
    }
}