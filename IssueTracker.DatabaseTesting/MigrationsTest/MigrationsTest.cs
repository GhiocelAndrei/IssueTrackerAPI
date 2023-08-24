using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using IssueTracker.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DatabaseTesting.MigrationsTest
{
    public class MigrationTests
    {
        private readonly string _connectionString;
        
        public MigrationTests()
        {
            var environment = Environment.GetEnvironmentVariable("IssueTrackerTestEnvironment") ?? "ci";

            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile($"appsettings.{environment}.json")
                            .Build();

            _connectionString = config.GetConnectionString("SqlServer");

            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            var options = new DbContextOptionsBuilder<DbContext>()
                              .UseSqlServer(_connectionString)
                              .Options;

            using var context = new DbContext(options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public void Migrations_Are_Idempotent()
        {
            var services = new ServiceCollection();
            services.SetUpFluentMigration(_connectionString);

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            runner.MigrateDown(0);

            runner.MigrateUp();
        }
    }

}
