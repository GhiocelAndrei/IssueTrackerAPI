using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Exceptions;
using IssueTracker.DataAccess.DatabaseContext;
using System.Transactions;

namespace IssueTracker.Application.Services
{
    public class SprintsService : BaseService<Sprint, CreateSprintCommand, UpdateSprintCommand>, ISprintsService
    {
        private readonly IIssuesService _issueService;
        public SprintsService(IssueContext dbContext, IMapper mapper, IIssuesService issueService) : base(dbContext, mapper)
        {
            _issueService = issueService;
        }

        public override async Task DeleteAsync(long id, CancellationToken ct)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _issueService.UnassignSprintFromIssuesAsync(id, ct);

            await base.DeleteAsync(id, ct);

            transactionScope.Complete();
        }

        public async Task CloseSprint(long id, CancellationToken ct)
        {
            var sprint = await base.GetAsync(id, ct);

            if (sprint == null)
                throw new NotFoundException("The requested entity could not be found.");

            sprint.Active = false;

            await DbContext.SaveChangesAsync(ct);
        }

        public async Task CreateSprintWithIssuesAsync(CreateSprintWithIssuesCommand sprintDto, CancellationToken ct)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var sprint = await base.CreateAsync(sprintDto.SprintDto, ct);

            await _issueService.AssignSprintToIssuesAsync(sprintDto.Ids, sprint.Id, ct);

            transactionScope.Complete();
        }


    }
}
