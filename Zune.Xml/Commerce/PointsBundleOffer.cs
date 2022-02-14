using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(nameof(EnumeratePointsBundlesResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class PointsBundleOffer
    {
        public string OfferId { get; set; }
        public string OfferName { get; set; }
        public string PriceText { get; set; }
        public bool IsTrial { get; set; }
        public int NumPoints { get; set; }
        public int WholePrice { get; set; }
        public int FractionalPrice { get; set; }
        public string TaxType { get; set; }
        public bool UserIsSubscribed { get; set; }

        public string Media { get; set; }
        public int PromoPoints { get; set; }
        public bool Subscription { get; set; }
        public bool Trial { get; set; }
    }
}