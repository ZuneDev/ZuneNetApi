using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.Endpoints
{
    public class Endpoints
    {
        [XmlElement("ENDPOINT")]
        public List<Endpoint> endpoints = new();
    }
}