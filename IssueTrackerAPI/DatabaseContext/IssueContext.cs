using Microsoft.EntityFrameworkCore;
using IssueTrackerAPI.Models;
using System.Reflection.Metadata;

namespace IssueTrackerAPI.DatabaseContext
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
        }
    }

}
