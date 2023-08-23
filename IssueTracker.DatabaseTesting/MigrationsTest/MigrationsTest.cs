using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using IssueTracker.Application.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IssueTracker.DatabaseTesting.MigrationsTest
{
    public class MigrationTests
    {
        private string _connectionString;
        private string _masterConnectionString;
        
        public MigrationTests()
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.ci.json")
                            .Build();
            
            _connectionString = config.GetConnectionString("ConnString");
            _masterConnectionString = config.GetConnectionString("MasterString");

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

            runner.MigrateDown(0);

            runner.MigrateUp();
        }
    }

}
