using Microsoft.Extensions.DependencyInjection;
using IssueTracker.DataAccess;

namespace IssueTracker.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection ApplicationAddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDataAccess(connectionString);
            return services;
        }
    }
}
