using Microsoft.Extensions.DependencyInjection;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;

namespace IssueTracker.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IssueContext>(
                o => o.UseSqlServer(connectionString));

            services.AddFluentMigratorCore()
            .ConfigureRunner(config => config
            .AddSqlServer()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(Migrations.AddRoleColumnToUsersTable).Assembly).For.All())
            .AddLogging(config => config.AddFluentMigratorConsole());

            return services;
        }
    }
}
