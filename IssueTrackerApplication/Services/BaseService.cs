using AutoMapper;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public abstract class BaseService<T, TCreateCommand, TUpdateCommand> : IBaseService<T, TCreateCommand, TUpdateCommand>
        where T : class
        where TCreateCommand : class
        where TUpdateCommand : class
    {
        protected readonly IssueContext DbContext;
        protected readonly IMapper Mapper;

        protected BaseService(IssueContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        public Task<List<T>> GetAllAsync(CancellationToken ct)
            => DbContext.Set<T>().ToListAsync(ct);

        public async Task<T> GetAsync(long id, CancellationToken ct)
        {
            var issue = await DbContext.Set<T>().FindAsync(id, ct);

            if (issue == null) 
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            return issue;
        }
        
        public async Task<T> UpdateAsync(long id, TUpdateCommand command, CancellationToken ct)
        {
            var entityModified = await GetAsync(id, ct);

            if (entityModified == null)
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            Mapper.Map(command, entityModified);

            if (entityModified is IModificationTracking modificationTrackingEntity)
            {
                modificationTrackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            await DbContext.SaveChangesAsync(ct);
            return entityModified;
        }

        public virtual async Task<T> CreateAsync(TCreateCommand command, CancellationToken ct)
        {
            var entity = Mapper.Map<T>(command);

            if (entity is ICreationTracking creationTrackingEntity)
            {
                creationTrackingEntity.CreatedAt = DateTime.UtcNow;
            }

            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = false;
            }

            DbContext.Set<T>().Add(entity);
            await DbContext.SaveChangesAsync(ct);
            return entity;
        }

        public virtual async Task DeleteAsync(long id, CancellationToken ct)
        {
            var entity = await GetAsync(id, ct);
            if (entity == null)
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            if (entity is ISoftDeletable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DeletedAt = DateTime.UtcNow;
            }
            else
            {
                DbContext.Set<T>().Remove(entity);
            }

            await DbContext.SaveChangesAsync(ct);
        }
    }
}
