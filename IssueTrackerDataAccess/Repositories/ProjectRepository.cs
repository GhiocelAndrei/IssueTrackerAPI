using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;

namespace IssueTracker.DataAccess.Repositories
{
    public class ProjectRepository : GenericRepositoryForEntitiesWithId<Project>
    {
        public ProjectRepository(IssueContext dbContext) : base(dbContext)
        {
        }
    }
}
