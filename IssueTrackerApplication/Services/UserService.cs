using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.Repositories;

namespace IssueTracker.Application.Services
{
    public class UserService : BaseService<User, CreateUserCommand, UpdateUserCommand>
    {
        public UserService(IGenericRepository<User> _repository, IMapper _mapper) : base(_repository, _mapper)
        {
        }

        public async Task<string> LoginUserAsync(CreateUserCommand userCommand)
        {
            var user = _mapper.Map<User>(userCommand);

            var userExists = await _repository
                .GetUniqueWithConditionAsync(anyUser => anyUser.Name == user.Name && anyUser.Email == user.Email);

            return userExists?.Role;
        }
    }
}
