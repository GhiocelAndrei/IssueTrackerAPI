using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Application.Services
{
    public class IssueService : BaseService<Issue, CreateIssueCommand, UpdateIssueCommand>
    {
        protected readonly IssueContext _dbContext;
        public IssueService(IssueContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override async Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken cancellationToken)
        {
            var reporterExists = await _dbContext.Set<User>()
                .AsNoTracking().AnyAsync(u => u.Id == entity.ReporterId, cancellationToken);

            if (!reporterExists)
            {
                throw new InvalidInputException("Given input is invalid");
            }

            var result = await base.CreateAsync(entity, cancellationToken);

            return result;
        }
    }
}
