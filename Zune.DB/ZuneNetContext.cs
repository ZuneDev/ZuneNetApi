using System.Data.Entity;
using Zune.DB.Models;

namespace Zune.Xml
{
    public class ZuneNetContext : DbContext
    {
        public ZuneNetContext() : base()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ZuneNetContext>());
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        //public DbSet<Badge> AvailableBadges { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

