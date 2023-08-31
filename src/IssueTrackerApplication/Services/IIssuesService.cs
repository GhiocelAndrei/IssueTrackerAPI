using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IIssuesService : IBaseService<Issue>
    {
        Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken ct);
        Task<List<Issue>> GetIssuesBySprintIdAsync(long id, CancellationToken ct);
        Task AssignSprintToIssuesAsync(List<long> ids, long sprintId, CancellationToken ct);
        Task UnassignSprintFromIssuesAsync(long sprintId, CancellationToken ct);
        Task<List<UserIssueDto>> GetIssuesForUserAsync(long projectId, long userId, CancellationToken ct);
    }
}
