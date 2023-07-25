using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Application.Services
{
    public class UserService : BaseService<User, CreateUserCommand, UpdateUserCommand>
    {
        public UserService(IGenericRepository<User> _repository, IMapper _mapper) : base(_repository, _mapper)
        {
        }

        public async Task<string> LoginUserAsync(LoginUserCommand userCommand)
        {
            var userExists = await _repository
                .GetUniqueWithConditionAsync(anyUser => anyUser.Name == userCommand.Name && anyUser.Email == userCommand.Email);

            if(userExists == null)
            {
                throw new NotFoundException("Login credentials are wrong");
            }

            return userExists.Role;
        }
    }
}
