using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;

namespace IssueTracker.DataAccess.Repositories
{
    public class IssueRepository : GenericRepositoryForEntitiesWithId<Issue>
    {
        protected readonly IGenericRepository<User> _userRepository;
        public IssueRepository(IssueContext dbContext, IGenericRepository<User> userRepository) : base(dbContext) 
        {
            _userRepository = userRepository;
        }

        public override async Task<Issue> AddAsync(Issue entity, CancellationToken cancellationToken)
        {
            var reporterExists = await _userRepository.ExistsWithConditionAsync(u => u.Id == entity.ReporterId, cancellationToken);

            if (!reporterExists)
            {
                return null;
            }

            var result = await base.AddAsync(entity, cancellationToken);

            return result;
        }
    }
}
