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
        public DbSet<Project> Project { get; set; } = null!;
        
    }

}
