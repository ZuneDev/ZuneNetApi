using Atom.Xml;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Zune.DB.Models.Joining;
using Zune.Xml.Commerce;

namespace Zune.DB.Models
{
    public class Member
    {
        public Member()
        {

        }

        public Member(Xml.SocialApi.Member xmlMember = null, SignInResponse signInResponse = null)
        {
            if (xmlMember != null)
                SetFromXmlMember(xmlMember);
            if (signInResponse != null)
                SetFromSignInResponse(signInResponse);
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string ZuneTag { get; set; }
        public int PlayCount { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }

        public IList<Link> Playlists { get; set; }
        public IList<MemberBadge> Badges { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<Message> Messages { get; set; }
        public IList<MemberMember> Friends { get; set; }

        public DateTime Updated { get; set; }

        public string Xuid { get; set; }
        public string Locale { get; set; }
        public bool ParentallyControlled { get; set; }
        public bool ExplicitPrivilege { get; set; }
        public bool Lightweight { get; set; }
        public Guid UserReadID { get; set; }
        public Guid UserWriteID { get; set; }
        public bool UsageCollectionAllowed { get; set; }

        public bool TagChangeRequired { get; set; }
        public bool AcceptedTermsOfService { get; set; }
        public bool AccountSuspended { get; set; }
        public bool SubscriptionLapsed { get; set; }
        public bool BillingUnavailable { get; set; }

        public double PointsBalance { get; set; }
        public double SongCreditBalance { get; set; }
        public string SongCreditRenewalDate { get; set; }

        public string SubscriptionOfferId { get; set; }
        public string SubscriptionRenewalOfferId { get; set; }
        public string BillingInstanceId { get; set; }
        public bool SubscriptionEnabled { get; set; }
        public bool SubscriptionBillingViolation { get; set; }
        public bool SubscriptionPendingCancel { get; set; }
        public string SubscriptionStartDate { get; set; }
        public string SubscriptionEndDate { get; set; }
        public string SubscriptionMeteringCertificate { get; set; }
        public string LastLabelTakedownDate { get; set; }
        public Tuner MediaTypeTunerRegisterInfo { get; set; }

        public Tuner TunerRegisterInfo { get; set; }

        public string UserTile { get; set; }
        public string Background { get; set; }


        public void SetFromSignInResponse(SignInResponse sir)
        {
            Xuid = sir.AccountInfo.Xuid;
            Locale = sir.AccountInfo.Locale;
            ParentallyControlled = sir.AccountInfo.ParentallyControlled;
            ExplicitPrivilege = sir.AccountInfo.ExplicitPrivilege;
            Lightweight = sir.AccountInfo.Lightweight;
            UserReadID = sir.AccountInfo.UserReadID;
            UserWriteID = sir.AccountInfo.UserWriteID;
            UsageCollectionAllowed = sir.AccountInfo.UsageCollectionAllowed;

            TagChangeRequired = sir.AccountState.TagChangeRequired;
            AcceptedTermsOfService = sir.AccountState.AcceptedTermsOfService;
            AccountSuspended = sir.AccountState.AccountSuspended;
            SubscriptionLapsed = sir.AccountState.SubscriptionLapsed;
            BillingUnavailable = sir.AccountState.BillingUnavailable;

            PointsBalance = sir.Balances.PointsBalance;
            SongCreditBalance = sir.Balances.SongCreditBalance;
            SongCreditRenewalDate = sir.Balances.SongCreditRenewalDate;

            SubscriptionOfferId = sir.SubscriptionInfo.SubscriptionOfferId;
            SubscriptionRenewalOfferId = sir.SubscriptionInfo.SubscriptionRenewalOfferId;
            BillingInstanceId = sir.SubscriptionInfo.BillingInstanceId;
            SubscriptionEnabled = sir.SubscriptionInfo.SubscriptionEnabled;
            SubscriptionBillingViolation = sir.SubscriptionInfo.SubscriptionBillingViolation;
            SubscriptionPendingCancel = sir.SubscriptionInfo.SubscriptionPendingCancel;
            SubscriptionStartDate = sir.SubscriptionInfo.SubscriptionStartDate;
            SubscriptionEndDate = sir.SubscriptionInfo.SubscriptionEndDate;
            SubscriptionMeteringCertificate = sir.SubscriptionInfo.SubscriptionMeteringCertificate;
            LastLabelTakedownDate = sir.SubscriptionInfo.LastLabelTakedownDate;
            MediaTypeTunerRegisterInfo = new Tuner(sir.SubscriptionInfo.MediaTypeTunerRegisterInfo);

            TunerRegisterInfo = new Tuner(sir.TunerRegisterInfo);
        }

        public SignInResponse GetSignInResponse()
        {
            return new SignInResponse
            {
                AccountState = new AccountState
                {
                    TagChangeRequired = TagChangeRequired,
                    AcceptedTermsOfService = AcceptedTermsOfService,
                    AccountSuspended = AccountSuspended,
                    SubscriptionLapsed = SubscriptionLapsed,
                    BillingUnavailable = BillingUnavailable
                },
                AccountInfo = new AccountInfo
                {
                    ZuneTag = ZuneTag,
                    Xuid = Xuid,
                    Locale = Locale,
                    ParentallyControlled = ParentallyControlled,
                    ExplicitPrivilege = ExplicitPrivilege,
                    Lightweight = Lightweight,
                    UserReadID = UserReadID,
                    UserWriteID = UserWriteID,
                    UsageCollectionAllowed = UsageCollectionAllowed,
                },
                Balances = new Balances
                {
                    PointsBalance = PointsBalance,
                    SongCreditBalance = SongCreditBalance,
                    SongCreditRenewalDate = SongCreditRenewalDate
                },
                SubscriptionInfo = new SubscriptionInfo
                {
                    SubscriptionOfferId = SubscriptionOfferId,
                    SubscriptionRenewalOfferId = SubscriptionRenewalOfferId,
                    BillingInstanceId = BillingInstanceId,
                    SubscriptionEnabled = SubscriptionEnabled,
                    SubscriptionBillingViolation = SubscriptionBillingViolation,
                    SubscriptionPendingCancel = SubscriptionPendingCancel,
                    SubscriptionStartDate = SubscriptionStartDate,
                    SubscriptionEndDate = SubscriptionEndDate,
                    SubscriptionMeteringCertificate = SubscriptionMeteringCertificate,
                    LastLabelTakedownDate = LastLabelTakedownDate,
                    MediaTypeTunerRegisterInfo = MediaTypeTunerRegisterInfo?.GetTunerRegisterInfo()
                },
                TunerRegisterInfo = TunerRegisterInfo?.GetTunerRegisterInfo()
            };
        }

        public void SetFromXmlMember(Xml.SocialApi.Member xmlMember)
        {
            ZuneTag = xmlMember.ZuneTag;
            PlayCount = xmlMember.PlayCount;
            DisplayName = xmlMember.DisplayName;
            Status = xmlMember.Status;
            Bio = xmlMember.Bio;
            Location = xmlMember.Location;
            Playlists = xmlMember.Playlists;
            UserTile = xmlMember.Images.FirstOrDefault(i => i.Title == "usertile")?.Href;
            Background = xmlMember.Images.FirstOrDefault(i => i.Title == "background")?.Href;
        }

        public Xml.SocialApi.Member GetXmlMember()
        {
            return new Xml.SocialApi.Member
            {
                Id = Id.ToString(),
                ZuneTag = ZuneTag,
                PlayCount = PlayCount,
                DisplayName = DisplayName,
                Status = Status,
                Bio = Bio,
                Location = Location,
                Images =
                {
                    new Link(UserTile, "enclosure")
                    {
                        Title = "userTile"
                    },
                    new Link(Background, "enclosure")
                    {
                        Title = "background"
                    },
                },
                Playlists = Playlists?.ToList(),
                Links =
                {
                    new Link($"https://socialapi.zune.net/{Locale}/members/{ZuneTag}"),
                    new Link($"https://socialapi.zune.net/{Locale}/members/{ZuneTag}/friends", "related")
                    {
                        Title = "friends"
                    },
                },
                Updated = Updated,
                Title = GetDisplayName(),
                Content = string.Empty,
                Author = new Author
                {
                    Name = ZuneTag,
                    Url = GetUrl()
                },

                Namespace = Atom.Constants.ZUNE_PROFILES_NAMESPACE,
            };
        }

        public string GetDisplayName() => string.IsNullOrEmpty(DisplayName) ? ZuneTag : DisplayName;

        public string GetUrl() => "http://social.zune.net/member/" + ZuneTag;

        public static Guid GetGuidFromZuneTag(string zuneTag)
        {
            // Compute 256-bit (32-byte) hash
            byte[] hash = System.Security.Cryptography.SHA256.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(zuneTag));
            
            // GUIDs are 128 bits (16 bytes), so we XOR the first 16 bytes of the hash with the last 16 bytes.
            // Will this cause collisions? Likely not. Should this be investigated before production? Probably.
            byte[] guid = new byte[16];
            for (int i = 0; i < guid.Length; i++)
                guid[i] = (byte)(hash[i] ^ hash[i + 16]);
            return new Guid(guid);
        }
    }
}
