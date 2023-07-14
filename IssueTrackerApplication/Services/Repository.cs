using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IssueContext _dbContext;

        public Repository(IssueContext dbContext)
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

        public async Task<(bool isSuccess, string message, T entity)> Add(T entity)
        {
            if (entity is Issue issueEntity)
            {
                var reporterExists = await _dbContext.Users.AnyAsync(u => u.Id == issueEntity.ReporterId);
                var assigneeExists = await _dbContext.Users.AnyAsync(u => u.Id == issueEntity.AssigneeId);

                if (!reporterExists || !assigneeExists)
                {
                    return (false, "ReporterId or AssigneeId doesn't exist in database.", null);
                }
            }

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
            return (true, string.Empty, entity);
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
    }
}
