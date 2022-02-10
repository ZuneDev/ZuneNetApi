using Atom;
using Atom.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class ArtistImageEntry : Entry
    {
        [XmlArray("instances", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<ImageInstance> Instances { get; set; }
    }
}
