using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class IssueRepository : GenericRepository<Issue>
    {
        public IssueRepository(IssueContext dbContext) : base(dbContext) 
        { 
        }

        public override async Task<Issue> Add(Issue entity)
        {
            var reporterExists = await _dbContext.Users.AnyAsync(u => u.Id == entity.ReporterId);

            if (!reporterExists)
            {
                return null;
            }

            var result = await base.Add(entity);

            return result;
        }
    }
}
