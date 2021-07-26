using System;
using System.Xml.Serialization;

namespace Atom.Xml
{
    [XmlRoot(ElementName = "entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class Entry
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "updated")]
        public DateTime? Updated { get; set; }

        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "link")]
        public Link Link { get; set; }

        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }
    }
}