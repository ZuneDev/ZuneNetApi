using Atom;
using Atom.Attributes;
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

        [XmlElement("longDescription")]
        public string LongDescription { get; set; }

        [XmlElement("shortDescription")]
        public string ShortDescription { get; set; }

        [XmlElement("podcastType")]
        public string PodcastType { get; set; }

        [XmlElement("sourceUrl")]
        public string SourceUrl { get; set; }

        [XmlElement("websiteUrl")]
        public string WebsiteUrl { get; set; }

        [XmlElement("releaseDate")]
        public DateTime EarliestAvailableDate { get; set; }

        [XmlArray("categories")]
        [XmlArrayItem("category")]
        public List<SeriesCategory> Categories { get; set; }
    }
}
