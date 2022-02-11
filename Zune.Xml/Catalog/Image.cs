using Atom;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("image", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class Image
    {
        [XmlArray("instances")]
        [XmlArrayItem("imageInstance")]
        public List<ImageInstance> Instances { get; set; }
    }
}
