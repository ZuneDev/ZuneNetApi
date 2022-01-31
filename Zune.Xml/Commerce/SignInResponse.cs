using Atom;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(nameof(SignInResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class SignInResponse
    {
        public AccountState AccountState { get; set; }
        public AccountInfo AccountInfo { get; set; }
        public Balances Balances { get; set; }
        public SubscriptionInfo SubscriptionInfo { get; set; }

        // TODO: this is a list.
        [XmlElement(IsNullable = true)]
        public TunerRegisterInfo TunerRegisterInfo { get; set; }
    }
}
