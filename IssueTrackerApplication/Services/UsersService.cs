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
         
        public async Task<string> LoginUserAsync(LoginUserCommand userCommand, CancellationToken ct)
        {
            var userExists = await DbContext.Set<User>()
                .Where(anyUser => anyUser.Name == userCommand.Name && anyUser.Email == userCommand.Email)
                .SingleOrDefaultAsync(ct);

            if (userExists == null)
            {
                throw new NotFoundException("Login credentials are wrong");
            }

            return userExists.Role;
        }
    }
}
