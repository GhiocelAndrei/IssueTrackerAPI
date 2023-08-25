using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using IssueTracker.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DatabaseTesting.MigrationsTest
{
    public class MigrationTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly DbContextOptions _dbContextOptions;

        public MigrationTests()
        {
            var environment = Environment.GetEnvironmentVariable("IssueTrackerTestEnvironment") ?? "ci";

            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile($"appsettings.{environment}.json")
                            .Build();

            _connectionString = config.GetConnectionString("SqlServer");

            _dbContextOptions = new DbContextOptionsBuilder<DbContext>()
                              .UseSqlServer(_connectionString)
                              .Options;

            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using var context = new DbContext(_dbContextOptions);
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            using var context = new DbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
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
