using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.DataAccess.DatabaseContext
{
    public class IssueContext : DbContext
    {
        public IssueContext(DbContextOptions<IssueContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Reporter)
                .WithMany()
                .HasForeignKey(i => i.ReporterId);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Assignee)
                .WithMany()
                .HasForeignKey(i => i.AssigneeId);

            // Filters for models that implement ISoftDeletable
            modelBuilder.Entity<Issue>().HasQueryFilter(i => !i.IsDeleted);
            modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
        }
    }

}
