using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    [XmlType(Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class TunerInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
    }
}
