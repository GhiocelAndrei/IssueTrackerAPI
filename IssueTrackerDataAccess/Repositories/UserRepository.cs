using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IssueTracker.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(IssueContext dbContext) : base(dbContext)
        {
        }
    }
}
