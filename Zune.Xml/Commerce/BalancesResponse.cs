using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(nameof(BalancesResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class BalancesResponse
    {
        public Balances Balances { get; set; }
    }
}
