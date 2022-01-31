using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    [XmlType(Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class AccountState
    {
        public uint SignInErrorCode { get; set; }
        public bool TagChangeRequired { get; set; }
        public bool AcceptedTermsOfService { get; set; }
        public bool AccountSuspended { get; set; }
        public bool SubscriptionLapsed { get; set; }
        public bool BillingUnavailable { get; set; }
    }
}