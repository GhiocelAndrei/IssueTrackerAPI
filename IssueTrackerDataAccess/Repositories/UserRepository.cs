using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;

namespace IssueTracker.DataAccess.Repositories
{
    public class UserRepository : GenericRepositoryForEntitiesWithId<User>
    {
        public UserRepository(IssueContext dbContext) : base(dbContext)
        {
        }
    }
}
