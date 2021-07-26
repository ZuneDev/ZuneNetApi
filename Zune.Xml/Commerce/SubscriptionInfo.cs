namespace Zune.Xml.Commerce
{
    public class SubscriptionInfo
    {
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
        public TunerRegisterInfo MediaTypeTunerRegisterInfo { get; set; }
    }
}