using Atom;
using Atom.Xml;
using System.Xml.Serialization;

namespace Zune.Xml.Inbox
{
    [XmlRoot(ElementName = "entry", Namespace = Constants.ATOM_NAMESPACE)]
    public class MessageDetails : Entry
    {
        [XmlElement("content", Namespace = "")]
        public string TextContent { get; set; }

        [XmlIgnore]
#if NETSTANDARD2_0_OR_GREATER
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
#endif
        public Link ReplyLink
        {
            get => Links.Find(l => l.Relation == "reply");
            set
            {
                Links.RemoveAll(l => l.Relation == "reply");
                value.Relation = "reply";
                Links.Add(value);
            }
        }

        [XmlIgnore]
#if NETSTANDARD2_0_OR_GREATER
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
#endif
        public Link AltLink
        {
            get => Links.Find(l => l.Relation == "alternate");
            set
            {
                Links.RemoveAll(l => l.Relation == "alternate");
                value.Relation = "alternate";
                Links.Add(value);
            }
        }

        [XmlElement("albumtitle", Namespace = "")]
        public string AlbumTitle { get; set; }

        [XmlElement("artistname", Namespace = "")]
        public string ArtistName { get; set; }

        [XmlElement("songtitle", Namespace = "")]
        public string SongTitle { get; set; }

        [XmlElement("tracknumber", Namespace = "")]
        public int TrackNumber { get; set; }

        [XmlElement("playlistname", Namespace = "")]
        public string PlaylistName { get; set; }

        [XmlElement("podcastname", Namespace = "")]
        public string PodcastName { get; set; }

        [XmlElement("podcasturl", Namespace = "")]
        public string PodcastUrl { get; set; }

        [XmlElement("usertile", Namespace = "")]
        public string UserTile { get; set; }

        [XmlElement("zunetag", Namespace = "")]
        public string ZuneTag { get; set; }
    }
}
