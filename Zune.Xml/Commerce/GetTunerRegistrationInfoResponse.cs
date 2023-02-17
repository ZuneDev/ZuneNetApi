using Atom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

// this class is not production-ready. The signin workflow needs to actually build this correctly
namespace Zune.Xml.Commerce
{
    public enum MediaTypeEnum
    {
        Subscription,
        AppStore,
    }

    public class TunerInfoDef
    {
        [XmlElement("ID")]
        [Required]
        public string TunerId { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("RegistrationDate")]
        public DateTime RegistrationDate { get; set; }
        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Version")]
        public string Version { get; set; }

    }
    public class MediaTypeTunerPair
    {
        [XmlArray("RegisteredType")]
        public MediaTypeEnum RegisteredType { get; set; }
        [XmlArray("TunerList")]
        public List<TunerInfoDef> TunerList { get; set; }
        [XmlArray("NextTunerTypeDeregistrationDate")]
        public List<KeyValuePair<string, DateTime>> NextTunerTypeDeregistrationDate { get; set; }
        [XmlArray("TunerTypeMaxRegistered")]
        public List<KeyValuePair<string, uint>> TunerTypeMaxRegistered { get; set; }
    }
    [XmlRoot(nameof(GetTunerRegistrationInfoResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class GetTunerRegistrationInfoResponse
    {
        [XmlArray("RegisteredTuners/MediaTypeTunerPair")]
        public List<MediaTypeTunerPair> MediaTypeTunerPair { get; set; }
    }
}