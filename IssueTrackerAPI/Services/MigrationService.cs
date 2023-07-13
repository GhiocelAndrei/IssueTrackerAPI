using FluentMigrator.Runner;

namespace IssueTrackerAPI.Services
{
    public class MigrationService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MigrationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // No-op
            return Task.CompletedTask;
        }
    }
}
