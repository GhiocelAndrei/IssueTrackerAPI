using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.DataAccess.Repositories
{
    public class IssueRepository : GenericRepository<Issue>
    {
        protected readonly IGenericRepository<User> _userRepository;
        public IssueRepository(IssueContext dbContext, IGenericRepository<User> userRepository) : base(dbContext) 
        {
            _userRepository = userRepository;
        }

        public override async Task<Issue> AddAsync(Issue entity)
        {
            var reporterExists = await _userRepository.ExistsWithConditionAsync(u => u.Id == entity.ReporterId);

            if (!reporterExists)
            {
                return null;
            }

            var result = await base.AddAsync(entity);

            return result;
        }
    }
}
