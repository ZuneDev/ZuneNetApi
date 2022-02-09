using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(ElementName = nameof(GetUserResponse), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class GetUserResponse
    {
        public object User { get; set; }
        public object UserGroup { get; set; }
    }
}
