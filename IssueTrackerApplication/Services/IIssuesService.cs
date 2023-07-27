using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IIssuesService
    {
        Task<List<Issue>> GetAllAsync(CancellationToken ct);
        Task<Issue> GetAsync(long id, CancellationToken ct);
        Task<Issue> UpdateAsync(long id, UpdateIssueCommand command, CancellationToken ct);
        Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
