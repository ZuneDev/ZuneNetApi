using System;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public abstract class MiniMedia
    {
        [XmlElement("id")]
        public Guid Id { get; set; }

        [XmlElement("name")]
        public string Title { get; set; }
    }

    [XmlRoot("artist")]
    public class MiniArtist : MiniMedia { }

    [XmlRoot("album")]
    public class MiniAlbum : MiniMedia { }
}
