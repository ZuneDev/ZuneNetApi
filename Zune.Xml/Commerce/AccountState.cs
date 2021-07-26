namespace Zune.Xml.Commerce
{
    public class AccountState
    {
        public int SignInErrorCode { get; set; }
        public bool TagChangeRequired { get; set; }
        public bool AcceptedTermsOfService { get; set; }
        public bool AccountSuspended { get; set; }
        public bool SubscriptionLapsed { get; set; }
        public bool BillingUnavailable { get; set; }
    }
}