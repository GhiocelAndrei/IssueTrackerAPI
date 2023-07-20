using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(IssueContext dbContext) : base(dbContext)
        {
        }

        public async Task<String> FindUserRole(String name, String email)
        {
            var user = await _dbContext.Set<User>().
                        Where(u => u.Name == name && u.Email == email).SingleAsync();

            return user.Role;
        }
    }
}
