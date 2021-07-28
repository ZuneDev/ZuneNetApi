using Atom.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Key]
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
                    TagChangeRequired = this.TagChangeRequired,
                    AcceptedTermsOfService = this.AcceptedTermsOfService,
                    AccountSuspended = this.AccountSuspended,
                    SubscriptionLapsed = this.SubscriptionLapsed,
                    BillingUnavailable = this.BillingUnavailable
                },
                AccountInfo = new AccountInfo
                {
                    ZuneTag = this.ZuneTag,
                    Xuid = this.Xuid,
                    Locale = this.Locale,
                    ParentallyControlled = this.ParentallyControlled,
                    ExplicitPrivilege = this.ExplicitPrivilege,
                    Lightweight = this.Lightweight,
                    UserReadID = this.UserReadID,
                    UserWriteID = this.UserWriteID,
                    UsageCollectionAllowed = this.UsageCollectionAllowed,
                },
                Balances = new Balances
                {
                    PointsBalance = this.PointsBalance,
                    SongCreditBalance = this.SongCreditBalance,
                    SongCreditRenewalDate = this.SongCreditRenewalDate
                },
                SubscriptionInfo = new SubscriptionInfo
                {
                    SubscriptionOfferId = this.SubscriptionOfferId,
                    SubscriptionRenewalOfferId = this.SubscriptionRenewalOfferId,
                    BillingInstanceId = this.BillingInstanceId,
                    SubscriptionEnabled = this.SubscriptionEnabled,
                    SubscriptionBillingViolation = this.SubscriptionBillingViolation,
                    SubscriptionPendingCancel = this.SubscriptionPendingCancel,
                    SubscriptionStartDate = this.SubscriptionStartDate,
                    SubscriptionEndDate = this.SubscriptionEndDate,
                    SubscriptionMeteringCertificate = this.SubscriptionMeteringCertificate,
                    LastLabelTakedownDate = this.LastLabelTakedownDate,
                    MediaTypeTunerRegisterInfo = this.MediaTypeTunerRegisterInfo?.GetTunerRegisterInfo()
                },
                TunerRegisterInfo = this.TunerRegisterInfo?.GetTunerRegisterInfo()
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
                Id = this.Id,
                ZuneTag = this.ZuneTag,
                PlayCount = this.PlayCount,
                DisplayName = this.DisplayName,
                Status = this.Status,
                Bio = this.Bio,
                Location = this.Location,
                Images =
                {
                    new Link
                    {
                        Relation = "enclosure",
                        Href = UserTile,
                        Title = "usertile"
                    },
                    new Link
                    {
                        Relation = "enclosure",
                        Href = Background,
                        Title = "background"
                    },
                },
                Playlists = this.Playlists != null ? Playlists.ToList() : null,
                Links =
                {
                    new Link
                    {
                        Relation = "self",
                        Type = "application/atom+xml",
                        Href = $"https://socialapi.zune.net/{Locale}/members/{ZuneTag}"
                    },
                    new Link
                    {
                        Relation = "related",
                        Type = "application/atom+xml",
                        Href = $"https://socialapi.zune.net/{Locale}/members/{ZuneTag}/friends",
                        Title = "friends"
                    },
                },
                Updated = Updated.ToString("O"),
                Title = new Content
                {
                    Type = "text",
                    Value = GetDisplayName()
                },
                Content = new Content
                {
                    Type = "html",
                    Value = ""
                },
                Author = new Author
                {
                    Name = ZuneTag,
                    Url = GetUrl()
                },

                Namespace = Atom.Constants.ZUNE_PROFILES_NAMESPACE,
            };
        }

        public string GetDisplayName() => String.IsNullOrEmpty(DisplayName) ? ZuneTag : DisplayName;

        public string GetUrl() => "http://social.zune.net/member/" + ZuneTag;
    }
}
