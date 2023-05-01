using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdqCd
{
    public class MdqDescriptionElement
    {
        [XmlElement("text")]
        public string Text;

        [XmlArrayItem("word")]
        public List<string> Words;
    }
}