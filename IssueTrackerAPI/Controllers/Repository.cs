using IssueTrackerAPI.DatabaseContext;
using IssueTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueTrackerAPI.Controllers
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(long id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(long id);
        Task<bool> Exists(long id);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IssueContext _dbContext;

        public Repository(IssueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Get(long id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);

            if (entity is ISoftDeletable softDeletableEntity)
            {
                if (!softDeletableEntity.IsDeleted)
                {
                    return entity;
                }

                return null; // or throw an exception, or however you want to handle this case
            }

            return entity;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var entities = await _dbContext.Set<T>().ToListAsync();

            var result = new List<T>();

            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable softDeletableEntity)
                {
                    if (!softDeletableEntity.IsDeleted)
                    {
                        result.Add(entity);
                    }
                }
                else
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        public async Task<T> Add(T entity)
        {
            if (entity is ICreationTracking creationTrackingEntity)
            {
                creationTrackingEntity.CreatedAt = DateTime.Now;
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
                modificationTrackingEntity.UpdatedAt = DateTime.Now;
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
                softDeletableEntity.DeletedAt = DateTime.Now;
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
