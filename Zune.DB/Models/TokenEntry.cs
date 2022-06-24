using MongoDB.Bson.Serialization.Attributes;

namespace Zune.DB.Models
{
    public class TokenEntry
    {
        public TokenEntry()
        {

        }

        public TokenEntry(string tokenHash, string userName)
        {
            TokenHash = tokenHash;
            UserName = userName;
        }

        [BsonId]
        public string TokenHash { get; set; }
        
        public string UserName { get; set; }
    }
}
