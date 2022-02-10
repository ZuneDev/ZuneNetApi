using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Atom.Xml
{
    [XmlRoot(ElementName = "link", Namespace = Constants.ATOM_NAMESPACE)]
    public class Link
    {
        public Link(string href, string relation = "self", string type = Constants.ATOM_MIMETYPE)
        {
            Href = href;
            Relation = relation;
            Type = type;
        }

        [XmlAttribute(AttributeName = "rel")]
        public string Relation { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "updated")]
        public DateTimeOffset Updated { get; set; }

        [XmlAttribute(AttributeName = "id")]
        [Key]
        public string Id { get; set; }

        public static implicit operator Link(string href)
        {
            return new Link(href);
        }
    }
}