using IssueTracker.Abstractions.Mapping;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly IssueContext _dbContext;

        public IssueRepository(IssueContext dbContext)
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

        public async Task<List<UserIssueDto>> GetIssuesForUserAsync(long projectId, long userId, CancellationToken ct)
        {
            var sql = $@"SELECT
	                        i.ExternalId,
	                        i.Title,
	                        i.CreatedAt,
	                        i.UpdatedAt,
	                        u.[Name],
	                        u.Email
                        FROM
	                        Issues i
	                        join Users u
		                        on i.AssigneeId = u.Id
	                        join Projects p
		                        on i.ProjectId = p.Id
                        WHERE
	                        i.IsDeleted = 0
	                        and p.IsDeleted = 0
	                        and ProjectId= @projectId
	                        and AssigneeId = @userId";

            var projectIdParam= $"{projectId}";
            var userIdParam = $"{userId}";

            return await _dbContext.Set<UserIssueDto>().FromSqlRaw(sql,
                new SqlParameter("@projectId", projectIdParam),
                new SqlParameter("@userId", userIdParam))
                .ToListAsync(ct);
        }
    }
}
