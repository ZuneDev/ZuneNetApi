using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zune.DB.Models.Joining;
using Zune.Xml.SocialApi;

namespace Zune.DB.Models
{
    public class Badge
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public BadgeType TypeId { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public Guid MediaId { get; set; }
        public string MediaType { get; set; }
        public string Description { get; set; }

        public IList<MemberBadge> Members { get; set; }
    }
}
