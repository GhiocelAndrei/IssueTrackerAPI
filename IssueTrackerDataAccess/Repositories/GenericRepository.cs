using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly IssueContext _dbContext;

        public GenericRepository(IssueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Get(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> Add(T entity)
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
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            if (entity is IModificationTracking modificationTrackingEntity)
            {
                modificationTrackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete(long id)
        {
            var entity = await Get(id);
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

            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Exists(long id)
        {
            return await _dbContext.Set<T>().AnyAsync(e => EF.Property<long>(e, "Id") == id);
        }

        public async Task<bool> ExistsWithCondition(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AsNoTracking().AnyAsync(predicate);
        }
    }
}
