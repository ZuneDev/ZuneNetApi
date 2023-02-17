using System.Xml.Serialization;
using Atom;

namespace Zune.Xml.Commerce
{
    [XmlRoot(nameof(SignInRequest), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class SignInRequest
    {
        public SignInRequest(){}

        public TunerInfo TunerInfo { get; set; }
    }
}
