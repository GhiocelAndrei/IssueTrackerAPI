using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class UserService : BaseService<User, CreateUserCommand, UpdateUserCommand>
    {
        public UserService(IssueContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<string> LoginUserAsync(LoginUserCommand userCommand, CancellationToken ct)
        {
            var userExists = await _dbContext.Set<User>()
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
