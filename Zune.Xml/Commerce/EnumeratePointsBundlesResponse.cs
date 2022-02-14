using Atom;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(nameof(EnumeratePointsBundlesResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class EnumeratePointsBundlesResponse
    {
        public List<PointsBundleOffer> PointsBundleOffers { get; set; }
    }
}
