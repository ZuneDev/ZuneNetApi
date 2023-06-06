using System.Xml.Serialization;

namespace Zune.Net.Mix
{
    [XmlRoot(ElementName = "entry")]
    public class VectorEntry
    {
        public VectorEntry() { }
        public VectorEntry(Guid mbid, IEnumerable<int> genreIds)
        {
            var genreList = string.Join(",", genreIds);
            Vector = $"0,{genreList}";
            ItemMbid = mbid;
        }

        [XmlElement(ElementName = "updated")]
        public DateTime Updated { get; set; } = DateTime.Now;

        [XmlElement(ElementName = "id")]
        public Guid ItemMbid { get; set; } = Guid.Empty;

        [XmlElement(ElementName = "context")]
        public int Context { get; set; } = 12;

        [XmlElement(ElementName = "schemeId")]
        public int SchemeId { get; set; } = 1;

        // the vector is... interesting. its a combo of generes by int, prepended with "0," for reasons.. Use the constructor to make one.
        [XmlElement(ElementName = "vector")]
        public string? Vector { get; set; }

        [XmlElement(ElementName = "expirationDate")]
        public DateTime ExpirationDate { get; set; } = DateTime.Now + TimeSpan.FromDays(14);
    }
}
