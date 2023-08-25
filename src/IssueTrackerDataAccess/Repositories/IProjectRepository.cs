namespace IssueTracker.DataAccess.Repositories
{
    public interface IProjectRepository
    {
        Task<long> GetIssueSequenceAsync(long projectId, CancellationToken ct);
    }
}
