using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.DataAccess.Repositories
{
    public interface ISearchRepository
    {
        Task<List<SearchResultDto>> SearchAsync(int queryLimit, string value, CancellationToken ct);
    }
}
