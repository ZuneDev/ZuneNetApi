using System;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public abstract class MiniMedia
    {
        [XmlElement("id")]
        public Guid Id { get; set; }

        //[XmlElement("title")]
        //public abstract string Title { get; set; }
    }

    [XmlRoot("artist")]
    public class MiniArtist : MiniMedia
    {
        [XmlElement("name")]
        public string Title { get; set; }
    }

    [XmlRoot("album")]
    public class MiniAlbum : MiniMedia
    {
        [XmlElement("title")]
        public string Title { get; set; }
    }
}
