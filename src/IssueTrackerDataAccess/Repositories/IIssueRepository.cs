using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.DataAccess.Repositories
{
    public interface IIssueRepository
    {
        Task<List<UserIssueDto>> GetIssuesForUserAsync(long projectId, long userId, CancellationToken ct);
    }
}
