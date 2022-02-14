using System;
using System.Xml.Serialization;

namespace Zune.Xml.Catalog
{
    [XmlRoot("right")]
    public class Right
    {
        [XmlElement("licenseType")]
        public string LicenseType { get; set; }

        [XmlElement("licenseRight")]
        public MediaRightsEnum LicenseRight { get; set; }

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
        public PriceTypeEnum CurrencyCode { get; set; }

        [XmlElement("displayPrice")]
        public string DisplayPrice { get; set; }

        [XmlElement("audioEncoding")]
        public AudioEncodingEnum AudioEncoding { get; set; }

        [XmlElement("audioLocale")]
        public string AudioLocale { get; set; }

        [XmlElement("SubtitleLocale")]
        public string SubtitleLocale { get; set; }

        [XmlElement("videoEncoding")]
        public string VideoEncoding { get; set; }

        [XmlElement("videoDefinition")]
        public VideoDefinitionEnum VideoDefinition { get; set; }

        [XmlElement("videoResolution")]
        public VideoResolutionEnum VideoResolution { get; set; }

        [XmlElement("clientType")]
        public ClientTypeEnum ClientType { get; set; }

        [XmlElement("mediaInstanceId")]
        public string MediaInstanceId { get; set; }

        [XmlElement("fileSize")]
        public int FileSize { get; set; }
    }
}