using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IUsersService
    {
        Task<List<User>> GetAllAsync(CancellationToken ct);
        Task<User> GetAsync(long id, CancellationToken ct);
        Task<User> UpdateAsync(long id, UpdateUserCommand command, CancellationToken ct);
        Task<User> CreateAsync(CreateUserCommand entity, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
        Task<bool> ExistsAsync(long Id, CancellationToken ct);
        Task<string> LoginUserAsync(LoginUserCommand userCommand, CancellationToken ct);
    }
}
