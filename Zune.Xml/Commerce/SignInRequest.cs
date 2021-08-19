using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    [XmlRoot(ElementName = nameof(SignInRequest), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class SignInRequest
    {
        public TunerInfo TunerInfo { get; set; }
    }
}
