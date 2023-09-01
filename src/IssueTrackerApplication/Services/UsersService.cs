using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace IssueTracker.Application.Services
{
    public class UsersService : BaseService<User>, IUsersService
    {
        public UsersService(IssueContext dbContext, 
            IMapper mapper,
            IValidatorFactory validatorFactory) 
            : base(dbContext, mapper, validatorFactory)
        {
        }

        public Task<bool> ExistsAsync(long id, CancellationToken ct)
            => DbContext.Set<User>().AsNoTracking().AnyAsync(u => u.Id == id, ct);

        public Task<User> GetUserByEmailAsync(string email, CancellationToken ct)
            => DbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}
