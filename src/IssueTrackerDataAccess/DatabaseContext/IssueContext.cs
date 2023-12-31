﻿using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Abstractions.Mapping;

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
        public DbSet<Sprint> Sprints { get; set; } = null!;
        public DbSet<ScalarLong> ScalarLong { get; set; }
        public DbSet<SearchResultDto> SearchResult { get; set; }
        public DbSet<UserIssueDto> UserIssue { get; set; }

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

            modelBuilder.Entity<ScalarLong>().HasNoKey();
            modelBuilder.Entity<SearchResultDto>().HasNoKey();
            modelBuilder.Entity<UserIssueDto>().HasNoKey();
        }
    }

}
