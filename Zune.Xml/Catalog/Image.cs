using Atom;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("image", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
    public class Image
    {
        [XmlElement("id", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public Guid Id { get; set; }

        [XmlArray("instances", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("imageInstance", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<ImageInstance> Instances { get; set; } = new List<ImageInstance>();

        public static Image FromSingleInstance(Guid id, string uri)
        {
            return new Image()
            {
                Id = id,
                Instances =
                {
                    new ImageInstance
                    {
                        Url = uri,
                        Id = id
                    }
                }
            };
        }
    }
}
