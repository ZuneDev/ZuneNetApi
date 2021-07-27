using Atom.Xml;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace Zune.Xml.Inbox
{
    [XmlRoot(ElementName = "entry", Namespace = "")]
    public class MessageRoot : Entry
    {
        [XmlElement("author", Namespace = "")]
        public Author From { get; set; }

        [XmlElement("type", Namespace = "")]
        public string Type { get; set; }

        [XmlIgnore]
#if NETSTANDARD2_0_OR_GREATER
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
#endif
        public Content Subject
        {
            get => Title;
            set => Title = value;
        }

        [XmlIgnore]
#if NETSTANDARD2_0_OR_GREATER
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
#endif
        public DateTime Received
        {
            get => Updated;
            set => Updated = value;
        }

        [XmlIgnore]
#if NETSTANDARD2_0_OR_GREATER
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
#endif
        public Link DetailsLink
        {
            get => Links.FirstOrDefault();
            set
            {
                Links.Clear();
                value.Relation = "alternate";
                Links.Add(value);
            }
        }

        [XmlElement("status", Namespace = "")]
        public string Status { get; set; }

        [XmlElement("wishlist", Namespace = "")]
        public bool Wishlist { get; set; }

        [XmlElement("mediaid", Namespace = "")]
        public Guid MediaId { get; set; }
    }
}
