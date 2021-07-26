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

        [XmlElement("image", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Image { get; set; }

        [XmlElement("type", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Type { get; set; }

        [XmlElement("mediaId", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public Guid MediaId { get; set; }

        [XmlElement("mediaType", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string MediaType { get; set; }

        [XmlElement("description", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
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
