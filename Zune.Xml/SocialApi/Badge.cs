using Atom;
using Atom.Xml;
using System.Xml.Serialization;

namespace Zune.Xml.SocialApi
{
    [XmlRoot(ElementName = "entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class Badge : Entry
    {
        [XmlElement("typeId", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public BadgeType TypeId { get; set; }

        [XmlElement("type", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Type { get; set; }

        [XmlElement("media", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public BadgeMedia Media { get; set; }

        [XmlIgnore]
        public string Description
        {
            get => Content?.Value;
            set => Content = value;
        }

        [XmlIgnore]
        public string Image
        {
            get => Links.Find(l => l.Relation == "enclosure").Href;
            set
            {
                Links.Clear();
                Links.Add(new Link(value, relation: "enclosure"));
            }
        }
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
