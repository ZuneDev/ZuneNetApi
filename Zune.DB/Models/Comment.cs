using System.ComponentModel.DataAnnotations;

namespace Zune.DB.Models
{
    public class Comment
    {
        public const string ID_PREFIX = "urn:x-zune-membercomment:";

        [Key]
        public string Id { get; set; }
        public Member Author { get; set; }
        public Member Recipient { get; set; }
        public string Content { get; set; }
    }
}
