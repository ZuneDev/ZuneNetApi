namespace Zune.DB.Models.Joining
{
    public class MemberComment
    {
        public string MemberId { get; set; }
        public Member Member { get; set; }

        public string CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
