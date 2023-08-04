using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IssueContext _dbContext;

        public ProjectRepository(IssueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> GetIssueSequenceAsync(long projectId, CancellationToken ct)
        {
            var projectIdParameter = new SqlParameter("@ProjectId", projectId);

            var sqlQuery = @"UPDATE Projects WITH(HOLDLOCK, ROWLOCK)
                           SET IssueSequence = IssueSequence + 1
                           OUTPUT inserted.IssueSequence AS [Value]
                           WHERE Id = @ProjectId";

            var issueSequenceResult = await _dbContext.ScalarLong.FromSqlRaw(sqlQuery, projectIdParameter).ToListAsync(ct);

            return issueSequenceResult.First().Value;
        }
    }
}
