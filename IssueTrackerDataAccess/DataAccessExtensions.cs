using Microsoft.Extensions.DependencyInjection;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IssueContext>(
                o => o.UseSqlServer(connectionString));

            return services;
        }
    }
}
