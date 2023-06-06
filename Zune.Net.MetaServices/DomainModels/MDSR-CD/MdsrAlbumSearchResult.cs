using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MdsrCd
{
    public class MdsrAlbumSearchResult
    {
        [XmlArray("SearchResult")]
        [XmlArrayItem("Result")]
        public List<MdsrAlbum> Results = new();
    }
}