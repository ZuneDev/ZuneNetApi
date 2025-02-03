using Atom;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("feed", Namespace = Constants.ATOM_NAMESPACE)]
    public class Album : Media
    {
        [XmlElement("image", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Image> Images { get; set; }

        [XmlElement("releaseDate", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("entry", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Track> Tracks { get; set; }
    }
}
