using Atom;
using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class PodcastSeries : Media
    {
        [XmlElement("image", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Image> Images { get; set; }

        [XmlElement("imageUrl", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string ImageUrl { get; set; }

        [XmlElement("contentType", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string Type { get; set; }

        [XmlElement("feedUrl", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string FeedUrl { get; set; }

        [XmlElement("websiteUrl", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public string WebsiteUrl { get; set; }

        [XmlElement("dateAdded", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("author", Namespace = Constants.ATOM_NAMESPACE)]
        public Author Author { get; set; }

        [XmlArray("categories", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        [XmlArrayItem("category", Namespace = Constants.ZUNE_CATALOG_MUSIC_NAMESPACE)]
        public List<Category> Categories { get; set; }
    }
}
