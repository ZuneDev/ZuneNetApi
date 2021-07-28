namespace Zune.DB.Models.Joining
{
    public class MemberMember
    {
        public string MemberAId { get; set; }
        public Member MemberA { get; set; }

        public string MemberBId { get; set; }
        public Member MemberB { get; set; }
    }
}
