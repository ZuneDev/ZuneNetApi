using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MDSR
{
    public class SearchResult
    {
        [XmlElement("Result")]
        public List<Result> Results;
    }
}