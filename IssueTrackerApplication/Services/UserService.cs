using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class UserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userRepository.GetAll();

            return users;
        }

        public async Task<User> GetUser(long id)
        {
            var user = await _userRepository.Get(id);

            return user; 
        }

        public async Task<User> PutUser(long id, UserCommandDto userCommand)
        {
            var userModified = await _userRepository.Get(id);

            if (userModified == null)
            {
                return null;
            }

            _mapper.Map(userCommand, userModified);

            try
            {
                await _userRepository.Update(userModified);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (userModified == null) 
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return userModified;
        }

        public async Task<User> PostUser(UserCommandDto userCommand)
        {
            var user = _mapper.Map<User>(userCommand);

            var createdUser = await _userRepository.Add(user);

            return createdUser;
        }

        public async Task<bool> LoginUser(UserCommandDto userCommand)
        {
            var user = _mapper.Map<User>(userCommand);

            return await _userRepository.ExistsWithCondition(anyUser => anyUser.Name == user.Name && anyUser.Email == user.Email);
        }

        public async Task DeleteUser(long id)
        {
            await _userRepository.Delete(id);
        }
    }
}
