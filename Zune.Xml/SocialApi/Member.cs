using Atom;
using Atom.Attributes;
using Atom.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.SocialApi
{
    [XmlRoot("entry", Namespace = Constants.ATOM_NAMESPACE)]
    [NamespacePrefix("zune", Constants.ZUNE_PROFILES_NAMESPACE)]
    public class Member : Root
    {
        [XmlElement("playcount", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public int PlayCount { get; set; }

        [XmlElement("zunetag", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string ZuneTag { get; set; }

        [XmlElement("displayname", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string DisplayName { get; set; }

        [XmlElement("status", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Status { get; set; }

        [XmlElement("bio", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Bio { get; set; }

        [XmlElement("location", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Location { get; set; }

        [XmlArray("images", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        [XmlArrayItem(ElementName = "link", Namespace = Constants.ATOM_NAMESPACE)]
        public List<Link> Images { get; set; } = new List<Link>();

        [XmlArray("playlists", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        [XmlArrayItem(ElementName = "link")]
        public List<Link> Playlists { get; set; } = new List<Link>();
    }
}
