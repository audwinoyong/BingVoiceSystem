namespace BingVoiceSystem.WebMVC.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BingDBEntities : DbContext
    {
        public BingDBEntities()
            : base("name=BingDB")
        {
        }

        public virtual DbSet<ApprovedRule> ApprovedRules { get; set; }
        public virtual DbSet<PendingRule> PendingRules { get; set; }
        public virtual DbSet<RejectedRule> RejectedRules { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovedRule>()
                .Property(e => e.Question)
                .IsUnicode(false);

            modelBuilder.Entity<ApprovedRule>()
                .Property(e => e.Answer)
                .IsUnicode(false);

            modelBuilder.Entity<PendingRule>()
                .Property(e => e.Question)
                .IsUnicode(false);

            modelBuilder.Entity<PendingRule>()
                .Property(e => e.Answer)
                .IsUnicode(false);

            modelBuilder.Entity<RejectedRule>()
                .Property(e => e.Question)
                .IsUnicode(false);

            modelBuilder.Entity<RejectedRule>()
                .Property(e => e.Answer)
                .IsUnicode(false);
        }
    }
}
