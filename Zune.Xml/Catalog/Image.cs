using Atom;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("image")]//, Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class Image
    {
        [XmlElement("id")]
        public Guid Id { get; set; }

        [XmlArray("instances", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("imageInstance")]
        public List<ImageInstance> Instances { get; set; }
    }
}
