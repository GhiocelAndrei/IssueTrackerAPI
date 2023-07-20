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

        public async Task<T> GetAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
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

        public async Task<T> UpdateAsync(T entity)
        {
            if (entity is IModificationTracking modificationTrackingEntity)
            {
                modificationTrackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(long id)
        {
            var entity = await GetAsync(id);
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

        public Task<bool> ExistsAsync(long id) 
            => _dbContext.Set<T>().AnyAsync(e => EF.Property<long>(e, "Id") == id);
        

        public Task<bool> ExistsWithConditionAsync(Expression<Func<T, bool>> predicate)
            => _dbContext.Set<T>().AsNoTracking().AnyAsync(predicate);
        
    }
}
