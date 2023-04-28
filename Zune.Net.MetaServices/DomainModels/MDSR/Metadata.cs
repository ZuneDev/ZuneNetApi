using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MDSR
{
    // irritating name to make it generate correctly
    [XmlRoot("METADATA")]
    public class MDSRCDMetadata
    {
        [XmlElement("MDSR-CD")] // of type (UIX) Album
        public MDSRCD mDSRcD {get; set;}
    }
}