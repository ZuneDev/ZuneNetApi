using System;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    public class Right
    {
        [XmlElement("licenseType")]
        public string LicenseType { get; set; }

        [XmlElement("licenseRight")]
        public string LicenseRight { get; set; }

        [XmlElement("providerName")]
        public string ProviderName { get; set; }

        [XmlElement("providerCode")]
        public string ProviderCode { get; set; }

        [XmlElement("offerId")]
        public Guid OfferId { get; set; }

        [XmlElement("price")]
        public double Price { get; set; }

        [XmlElement("originalPrice")]
        public double OriginalPrice { get; set; }

        [XmlElement("currencyCode")]
        public string CurrencyCode { get; set; }

        [XmlElement("displayPrice")]
        public string DisplayPrice { get; set; }

        [XmlElement("audioEncoding")]
        public string AudioEncoding { get; set; }

        [XmlElement("audioLocale")]
        public string AudioLocale { get; set; }

        [XmlElement("SubtitleLocale")]
        public string SubtitleLocale { get; set; }

        [XmlElement("videoEncoding")]
        public string VideoEncoding { get; set; }

        [XmlElement("videoDefinition")]
        public string VideoDefinition { get; set; }

        [XmlElement("videoResolution")]
        public string VideoResolution { get; set; }

        [XmlElement("clientType")]
        public string ClientType { get; set; }
    }
}