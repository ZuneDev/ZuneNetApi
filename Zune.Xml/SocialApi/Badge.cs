using Atom;
using Atom.Xml;
using System;
using System.Xml.Serialization;

namespace Zune.Xml.SocialApi
{
    [XmlRoot(ElementName = "entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class Badge : Entry
    {
        [XmlElement("typeId", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public BadgeType TypeId { get; set; }

        [XmlElement("link", Namespace = Constants.ATOM_NAMESPACE)]
        public Link Image { get; set; }

        [XmlElement("type", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Type { get; set; }

        [XmlElement("media", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public BadgeMedia Media { get; set; }

        [XmlElement("content", Namespace = Constants.ATOM_NAMESPACE)]
        public string Description { get; set; }
    }

    public enum BadgeType
    {
        Invalid = -1,

        ActiveArtistListener_Gold,
        ActiveArtistListener_Bronze,
        ActiveArtistListener_Silver,

        ActiveAlbumListener_Gold,
        ActiveAlbumListener_Silver,
        ActiveAlbumListener_Bronze,

        ActiveForumsBadge_Gold,
        ActiveForumsBadge_Bronze,
        ActiveForumsBadge_Silver,

        ActiveReviewBadge_Gold,
        ActiveReviewBadge_Bronze,
        ActiveReviewBadge_Silver,
    }
}
