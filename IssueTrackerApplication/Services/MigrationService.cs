using FluentMigrator.Runner;
using IssueTracker.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IssueTracker.Application.Services
{
    public static class MigrationService
    {
        public static bool StartMigration(this IServiceCollection services, string connectionString)
        {
            var serviceProvider = services.AddFluentMigratorCore()
            .ConfigureRunner(config => config
            .AddSqlServer()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(IssueTracker.DataAccess.Migrations.AddRoleColumnToUsersTable).Assembly).For.All())
            .AddLogging(config => config.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

            using (var scope = serviceProvider.CreateScope())
            {
                try
                {
                    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    runner.MigrateUp();
                }
                catch (Exception ex)
                {
                    // Migration Failed
                    return false;
                }
            }

            return true;
        }
    }
}
