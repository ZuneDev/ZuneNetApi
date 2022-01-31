using System;
using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    [XmlType(Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class AccountInfo
    {
        public string ZuneTag { get; set; }
        public string Xuid { get; set; }
        public string Locale { get; set; }
        public bool ParentallyControlled { get; set; }
        public bool ExplicitPrivilege { get; set; }
        public bool Lightweight { get; set; }
        public Guid UserReadID { get; set; }
        public Guid UserWriteID { get; set; }
        public bool UsageCollectionAllowed { get; set; }
    }
}