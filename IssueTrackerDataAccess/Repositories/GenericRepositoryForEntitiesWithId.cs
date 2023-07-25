using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class GenericRepositoryForEntitiesWithId<T> : GenericRepository<T> where T : class, IHasId
    {
        protected GenericRepositoryForEntitiesWithId(IssueContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken)
            => _dbContext.Set<T>().AnyAsync(e => e.Id == id, cancellationToken);
    }
}
