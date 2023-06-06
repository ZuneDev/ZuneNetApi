using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdsrCd
{
    // irritating name to make it generate correctly
    [XmlRoot("METADATA")]
    public class MdsrAlbumRequestMetadata
    {
        [XmlElement("MDSR-CD")] // of type (UIX) Album
        public MdsrAlbumSearchResult mDSRcD {get; set;} = new();
    }
}