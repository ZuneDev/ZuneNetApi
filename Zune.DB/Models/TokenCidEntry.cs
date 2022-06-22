using MongoDB.Bson.Serialization.Attributes;

namespace Zune.DB.Models
{
    public class TokenCidEntry
    {
        public TokenCidEntry()
        {

        }

        public TokenCidEntry(string tokenHash, string cid)
        {
            TokenHash = tokenHash;
            Cid = cid;
        }

        [BsonId]
        public string TokenHash { get; set; }
        
        public string Cid { get; set; }
    }
}
