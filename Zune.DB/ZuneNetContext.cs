using Microsoft.EntityFrameworkCore;
using System;
using Zune.DB.Models;
using Zune.DB.Models.Joining;

namespace Zune.DB
{
    public class ZuneNetContext : DbContext
    {
        public string DbPath { get; private set; }

        public DbSet<Member> Members { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Badge> AvailableBadges { get; set; }
        public DbSet<Tuner> Tuners { get; set; }

        public ZuneNetContext(bool reset = false)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Combine(path, "Zune.Net", "zunenet.db");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DbPath));

            if (reset)
                Database.EnsureDeleted();
            Database.Migrate();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=" + DbPath);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberMember>().HasKey(mm => new { mm.MemberAId, mm.MemberBId });
            modelBuilder.Entity<MemberMember>()
                .HasOne(mm => mm.MemberA)
                .WithMany(m => m.Friends)
                .HasForeignKey(m => m.MemberAId);
            //modelBuilder.Entity<MemberMember>()
            //    .HasOne(mm => mm.MemberB)
            //    .WithMany(m => m.Friends)
            //    .HasForeignKey(m => m.MemberBId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Comments)
                .WithOne(c => c.Recipient)
                .HasForeignKey(c => c.Id);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Messages)
                .WithOne(msg => msg.Recipient)
                .HasForeignKey(msg => msg.Id);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.TunerRegisterInfo)
                .WithOne(t => t.Member)
                .HasForeignKey<Tuner>(t => t.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberBadge>().HasKey(mb => new { mb.MemberId, mb.BadgeId });
            modelBuilder.Entity<MemberBadge>()
                .HasOne(mb => mb.Member)
                .WithMany(m => m.Badges)
                .HasForeignKey(m => m.MemberId);
            modelBuilder.Entity<MemberBadge>()
                .HasOne(mb => mb.Badge)
                .WithMany(b => b.Members)
                .HasForeignKey(m => m.BadgeId);
        }
    }
}

