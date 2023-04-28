using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MDSR
{
    public class MDSRCD
    {
        [XmlElement]
        public string ReturnCode { get; set; } = "SUCCESS"; // default to success, we don't know what nothing looks like. I've seen rumors of an empty MDAR-CD element.

        [XmlElement]
        public SearchResult SearchResult { get; set; }
    }
}