using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class SeriesCategory
    {
        [XmlElement("categoryId")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Title { get; set; }
    }

    [XmlRoot("category")]
    public class PodcastCategory
    {
        [XmlElement("categoryId")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Title { get; set; }

        [XmlElement("subcategories")]
        [XmlArrayItem("subcategory")]
        public List<SeriesCategory> Subcategories { get; set; }
    }
}