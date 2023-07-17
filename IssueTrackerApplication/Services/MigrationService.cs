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
    }
}
