using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IUsersService : IBaseService<User>
    {
        Task<bool> ExistsAsync(long id, CancellationToken ct);
        Task<User> GetUserByEmailAsync(string email, CancellationToken ct);
    }
}
