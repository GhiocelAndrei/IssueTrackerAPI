using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface ISprintsService : IBaseService<Sprint>
    {
        Task CloseSprint(long id, CancellationToken ct);

        Task CreateSprintWithIssuesAsync(CreateSprintWithIssuesCommand sprintDto, CancellationToken ct);
    }
}
