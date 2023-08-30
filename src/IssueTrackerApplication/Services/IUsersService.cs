using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IUsersService : IBaseService<User>
    {
        Task<bool> ExistsAsync(long id, CancellationToken ct);
        Task<string> LoginUserAsync(LoginUserCommand userCommand, CancellationToken ct);
    }
}
