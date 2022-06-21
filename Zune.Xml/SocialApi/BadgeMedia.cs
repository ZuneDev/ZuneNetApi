using Atom;
using System;
using System.Xml.Serialization;

namespace Zune.Xml.SocialApi
{
    public class BadgeMedia
    {
        [XmlElement("id", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public Guid Id { get; set; }

        [XmlElement("type", Namespace = Constants.ZUNE_PROFILES_NAMESPACE)]
        public string Type { get; set; }
    }
}
