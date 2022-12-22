using Atom;
using Atom.Attributes;
using Atom.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot(ElementName = "podcast", Namespace = Constants.XBOX_LIVE_NAMESPACE)]
    [NamespacePrefix("live", Constants.XBOX_LIVE_NAMESPACE)]
    public class PodcastSeries : Media
    {
        [XmlElement("image")]
        public List<Image> Images { get; set; }

        [XmlElement("imageUrl")]
        public string ImageUrl { get; set; }

        [XmlElement("contentType")]
        public string Type { get; set; }

        [XmlElement("feedUrl")]
        public string FeedUrl { get; set; }

        [XmlElement("websiteUrl")]
        public string WebsiteUrl { get; set; }

        [XmlElement("dateAdded")]
        public DateTime ReleaseDate { get; set; }

        [XmlElement("author")]
        public Author Author { get; set; }

        [XmlArray("categories")]
        [XmlArrayItem("category")]
        public List<Category> Categories { get; set; }
    }
}
