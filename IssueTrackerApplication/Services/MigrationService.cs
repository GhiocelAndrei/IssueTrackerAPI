using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.Services
{
    public static class MigrationService
    {
        public static IServiceCollection SetUpFluentMigration(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(config => config
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(IssueTracker.DataAccess.Migrations.AddRoleColumnToUsersTable).Assembly).For.All())
                .AddLogging(config => config.AddFluentMigratorConsole());

            return services;
        }

        public static IServiceProvider StartMigrations(this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            var migratorRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

            try
            {
                migratorRunner.MigrateUp();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration failed with exception: {ex.Message}");
                throw;
            }

            return provider;
        }
    }
}
