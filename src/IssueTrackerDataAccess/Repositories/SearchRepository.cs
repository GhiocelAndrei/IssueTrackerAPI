using IssueTracker.Abstractions.Mapping;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IssueContext _dbContext;

        public SearchRepository(IssueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SearchResultDto>> SearchAsync(int queryLimit, string value, CancellationToken ct)
        {
            var sql = $@"SELECT TOP(@limit) {(int)SearchResultType.Issue} AS Type, i.Title AS TitleOrName 
                        FROM Issues i 
                        WHERE i.IsDeleted = 0 
                        AND (i.Title LIKE @keyword OR i.Description LIKE @keyword)

                        UNION ALL

                        SELECT TOP(@limit) {(int)SearchResultType.Project} AS Type, p.Name AS TitleOrName 
                        FROM Projects p 
                        WHERE p.Name LIKE @keyword";

            var keyword = $"{value}%";

            return await _dbContext.Set<SearchResultDto>().FromSqlRaw(sql,
                new SqlParameter("@limit", queryLimit),
                new SqlParameter("@keyword", keyword))
                .ToListAsync(ct);
        }
    }
}
