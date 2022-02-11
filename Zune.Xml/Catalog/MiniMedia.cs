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

    public class MiniArtist : MiniMedia { }

    public class MiniAlbum : MiniMedia { }
}
