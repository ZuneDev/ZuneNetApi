using Atom.Xml;
using System;
using System.ComponentModel.DataAnnotations;

namespace Zune.DB.Models
{
    public class Message
    {
        public const string ID_PREFIX = "urn:x-zune-inboxmessage:";

        public Message() { }

        public Message(Xml.Inbox.MessageRoot root = null, Xml.Inbox.MessageDetails details = null)
        {
            if (root != null)
                SetFromMessageRoot(root);
            if (details != null)
                SetFromMessageDetails(details);
        }

        private Link _ReplyLink;
        private Link _AltLink;
        private Link _DetailsLink;

        [Key]
        public string Id { get; set; }
        public virtual Member Sender { get; set; }
        public virtual Member Recipient { get; set; }

        public string TextContent { get; set; }
        public Link ReplyLink
        {
            get => _ReplyLink;
            set
            {
                value.Relation = "reply";
                _ReplyLink = value;
            }
        }
        public Link AltLink
        {
            get => _AltLink;
            set
            {
                value.Relation = "alternate";
                _AltLink = value;
            }
        }
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
        public string SongTitle { get; set; }
        public int TrackNumber { get; set; }
        public string PlaylistName { get; set; }
        public string PodcastName { get; set; }
        public string PodcastUrl { get; set; }
        //public string UserTile { get; set; }
        //public string ZuneTag { get; set; }

        public string Type { get; set; }
        public string Subject { get; set; }
        public DateTime Received { get; set; }
        //public Link DetailsLink
        //{
        //    get => _DetailsLink;
        //    set
        //    {
        //        value.Relation = "alternate";
        //        _DetailsLink = value;
        //    }
        //}
        public string DetailsLink { get; set; }
        public string Status { get; set; }
        public bool Wishlist { get; set; }
        public Guid MediaId { get; set; }

        public void SetFromMessageRoot(Xml.Inbox.MessageRoot root)
        {
            // TODO: Set Sender property from member ID only?

            Type = root.Type;
            Subject = root.Subject.Value;
            Received = root.Received;
            DetailsLink = root.DetailsLink.Href;
            Status = root.Status;
            Wishlist = root.Wishlist;
            MediaId = root.MediaId;
        }

        public Xml.Inbox.MessageRoot GetMessageRoot()
        {
            return new Xml.Inbox.MessageRoot
            {
                Id = this.Id,
                From = new Author
                {
                    Name = Sender.GetDisplayName(),
                    Url = Sender.GetUrl()
                },
                Type = this.Type,
                Subject = new Content
                {
                    Type = "text",
                    Value = Subject
                },
                Received = this.Received,
                DetailsLink = new Link
                {
                    Href = GetDetailsLink()
                },
                Status = this.Status,
                Wishlist = this.Wishlist,
                MediaId = this.MediaId,
            };
        }

        public void SetFromMessageDetails(Xml.Inbox.MessageDetails details)
        {
            TextContent = details.TextContent;
            ReplyLink = details.ReplyLink;
            AltLink = details.AltLink;
            AlbumTitle = details.AlbumTitle;
            ArtistName = details.ArtistName;
            SongTitle = details.SongTitle;
            TrackNumber = details.TrackNumber;
            PlaylistName = details.PlaylistName;
            PodcastName = details.PodcastName;
            PodcastUrl = details.PodcastUrl;
        }

        public Xml.Inbox.MessageDetails GetMessageDetails()
        {
            return new Xml.Inbox.MessageDetails
            {
                Id = this.Id,
                Title = new Content
                {
                    Type = "text",
                    Value = Subject
                },
                UserTile = this.Sender.UserTile,
                ZuneTag = this.Sender.ZuneTag,
                TextContent = this.TextContent,
                ReplyLink = this.ReplyLink,
                AltLink = this.AltLink,
                AlbumTitle = this.AlbumTitle,
                ArtistName = this.ArtistName,
                SongTitle = this.SongTitle,
                TrackNumber = this.TrackNumber,
                PlaylistName = this.PlaylistName,
                PodcastName = this.PodcastName,
                PodcastUrl = this.PodcastUrl,
            };
        }

        public string GetDetailsLink()
        {
            if (string.IsNullOrEmpty(DetailsLink))
                return $"https://inbox.zune.net/{Recipient.ZuneTag}/messaging/{Recipient.ZuneTag}/inbox/details/{Id}";
            else
                return DetailsLink;
        }
    }
}
