using AutoMapper;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Models;
using Microsoft.AspNetCore.JsonPatch;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using System.Text;

namespace IssueTracker.Application.Services
{
    public abstract class BaseService<T> : IBaseService<T>
        where T : class
    {
        protected readonly IssueContext DbContext;
        protected readonly IMapper Mapper;
        private readonly IValidatorFactory _validatorFactory;

        protected BaseService(IssueContext dbContext, 
            IMapper mapper,
            IValidatorFactory validatorFactory)
        {
            DbContext = dbContext;
            Mapper = mapper;
            _validatorFactory = validatorFactory;
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

        public async Task<T> UpdateAsync<TUpdateCommand>(long id, TUpdateCommand command, CancellationToken ct)
            where TUpdateCommand : class
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

        public async Task<T> PatchAsync<TUpdateDTO>(long id, JsonPatchDocument<TUpdateDTO> patchDTO, CancellationToken ct)
            where TUpdateDTO : class
        {
            var entityModified = await GetAsync(id, ct);

            if (entityModified == null)
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            var updateDTO = Mapper.Map<TUpdateDTO>(entityModified);

            try
            {
                patchDTO.ApplyTo(updateDTO);
            }
            catch (JsonPatchException)
            {
                throw new InvalidInputException("Invalid path in PATCH body.");
            }

            var validator = _validatorFactory.GetValidator<TUpdateDTO>();
            if(validator == null)
            {
                throw new MissingValidatorException("Received PATCH body that can't be validated.");
            }
            
            var validationResult = await validator.ValidateAsync(updateDTO, ct);
            if (!validationResult.IsValid)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Invalid value in PATCH body.");

                foreach (var error in validationResult.Errors)
                {
                    sb.AppendLine($"Property: {error.PropertyName}");
                    sb.AppendLine($"Error: {error.ErrorMessage}");
                    sb.AppendLine();
                }

                throw new InvalidInputException(sb.ToString());
            }
            
            
            Mapper.Map(updateDTO, entityModified);

            if (entityModified is IModificationTracking modificationTrackingEntity)
            {
                modificationTrackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            await DbContext.SaveChangesAsync(ct);

            return entityModified;
        }

        public virtual async Task<T> CreateAsync<TCreateCommand>(TCreateCommand command, CancellationToken ct)
            where TCreateCommand : class
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
