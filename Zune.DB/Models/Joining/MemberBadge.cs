namespace Zune.DB.Models.Joining
{
    public class MemberBadge
    {
        public string MemberId { get; set; }
        public Member Member { get; set; }

        public string BadgeId { get; set; }
        public Badge Badge { get; set; }
    }
}
