using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    public enum TunerRegisterType
    {
        Subscription,
        AppStore
    }

    [XmlType(Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class MediaTypeTunerRegisterInfo : TunerRegisterInfo
    {
        public TunerRegisterType RegisterType { get; set; }
        public bool Activated { get; set; }
        public bool Activable { get; set; }
    }
}
