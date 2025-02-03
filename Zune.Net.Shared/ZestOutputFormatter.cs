using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Atom;
using Atom.Attributes;
using Microsoft.AspNetCore.Mvc.Formatters;
using Zune.Xml;

namespace Zune.Net
{
    public class ZestOutputFormatter : XmlSerializerOutputFormatter
    {
        private Dictionary<Type, XmlSerializerNamespaces> _namespaceCache
            = new Dictionary<Type, XmlSerializerNamespaces>();

        public override IReadOnlyList<string> GetSupportedContentTypes(string contentType, Type objectType)
        {
            return base.GetSupportedContentTypes(contentType, objectType);
        }

        protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object value)
        {
            if (value is null)
            {
                base.Serialize(xmlSerializer, xmlWriter, value);
                return;
            }

            var ns = new XmlSerializerNamespaces();
            ns.Add("a", Atom.Constants.ATOM_NAMESPACE);
            ns.Add("m", Atom.Constants.ZUNE_CATALOG_MUSIC_NAMESPACE);
            ns.Add("c", Atom.Constants.ZUNE_COMMERCE_NAMESPACE);
            ns.Add("p", Atom.Constants.ZUNE_PROFILES_NAMESPACE);

            xmlSerializer.Serialize(xmlWriter, value, ns);
        }
    }
}
