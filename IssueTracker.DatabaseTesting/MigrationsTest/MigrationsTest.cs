using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using IssueTracker.Application.Services;
using Microsoft.Data.SqlClient;

namespace IssueTracker.DatabaseTesting.MigrationsTest
{
    public class MigrationTests
    {
        private string _connectionString;
        private string _masterConnectionString;

        public MigrationTests()
        {
            _connectionString = "Server=localhost,1433;Database=TestIssueTracker;User Id=sa;Password=1qazXSW@;TrustServerCertificate=true";
            _masterConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=1qazXSW@;TrustServerCertificate=true";

            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using (var connection = new SqlConnection(_masterConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand($"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TestIssueTracker') CREATE DATABASE TestIssueTracker;", connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
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
        }
    }

}
