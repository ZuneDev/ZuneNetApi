using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Atom.Attributes;

namespace Atom.Xml
{
    [XmlRoot(ElementName = "entry", Namespace = Constants.ATOM_NAMESPACE)]
    [NamespacePrefix("a", Constants.ATOM_NAMESPACE)]
    public class Entry
    {
        [XmlElement(ElementName = "title")]
        public Content Title { get; set; } = "List Of Items";

        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; }

#if NETSTANDARD
        [System.ComponentModel.DataAnnotations.Key]
#endif
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "link")]
        public List<Link> Links { get; set; } = new List<Link>();

        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }


        [XmlElement(ElementName = "content")]
        public Content Content { get; set; }
    }
}