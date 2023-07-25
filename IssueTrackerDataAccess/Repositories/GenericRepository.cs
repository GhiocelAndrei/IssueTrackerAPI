using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IssueContext _dbContext;

        protected GenericRepository(IssueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<T> GetAsync(long id, CancellationToken cancellationToken)
            => _dbContext.Set<T>().FindAsync(id, cancellationToken);

        public Task<T> GetUniqueWithConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            => _dbContext.Set<T>().Where(predicate).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
            => _dbContext.Set<T>().ToListAsync(cancellationToken);        

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity is ICreationTracking creationTrackingEntity)
            {
                creationTrackingEntity.CreatedAt = DateTime.UtcNow;
            }

            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = false;
            }

            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity is IModificationTracking modificationTrackingEntity)
            {
                modificationTrackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<T> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var entity = await GetAsync(id, cancellationToken);
            if (entity == null)
            {
                return null;
            }

            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DeletedAt = DateTime.UtcNow;
            }
            else
            {
                _dbContext.Set<T>().Remove(entity);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
        
        public Task<bool> ExistsWithConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            => _dbContext.Set<T>().AsNoTracking().AnyAsync(predicate, cancellationToken);
        
    }
}
