using Atom;
using System.Xml.Serialization;

namespace Zune.Xml.Commerce
{
    [XmlRoot(ElementName = nameof(GetUserRequest), Namespace = Constants.ZUNE_COMMERCE_NAMESPACE)]
    public class GetUserRequest
    {
    }
}
