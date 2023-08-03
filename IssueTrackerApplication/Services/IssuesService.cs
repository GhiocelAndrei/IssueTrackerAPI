using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.Abstractions.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using FluentValidation;

namespace IssueTracker.Application.Services
{
    public class IssuesService : BaseService<Issue>, IIssuesService
    {
        private readonly IUsersService _userService;
        private readonly IProjectsService _projectService;
        private readonly IUnitOfWork _transactionUnit;

        public IssuesService(IssueContext dbContext,
            IMapper mapper,
            IValidatorFactory validatorFactory,
            IUsersService userService,
            IProjectsService projectService,
            IUnitOfWork transactionUnit)
            : base(dbContext, mapper, validatorFactory)
        {
            _userService = userService;
            _projectService = projectService;
            _transactionUnit = transactionUnit;
        }

        public async Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken ct) 
        {
            if (!await _projectService.ExistsAsync(entity.ProjectId, ct))
            {
                throw new InvalidInputException("Project ID does not exits");
            }

            if (!await _userService.ExistsAsync(entity.ReporterId, ct))
            {
                throw new InvalidInputException("Reporter ID does not exits");
            }

            if (!await _userService.ExistsAsync(entity.AssigneeId, ct))
            {
                throw new InvalidInputException("Assignee ID does not exits");
            }

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var issueSequence = await _projectService.GetIssueSequenceAsync(entity.ProjectId, ct);
            var projectCode = await _projectService.GetProjectCodeAsync(entity.ProjectId, ct);

            entity.ExternalId = $"{projectCode}-{issueSequence}";
            var resultedIssue = await base.CreateAsync(entity, ct);

            transactionScope.Complete();

            return resultedIssue;
        }

        public async Task AssignSprintToIssuesAsync(List<long> ids, long sprintId, CancellationToken ct)
        {
            await _transactionUnit.ExecuteWithTransactionAsync(async () =>
            {
                var issuesModified = await DbContext.Issues
                .Where(issue => ids.Contains(issue.Id)).ToListAsync(ct);

                if (issuesModified.Count < ids.Count)
                    throw new InvalidInputException("One of the provided Ids does not correspond to any existing Issue.");

                foreach (var issue in issuesModified)
                {
                    issue.SprintId = sprintId;
                    issue.UpdatedAt = DateTime.UtcNow;
                }

                await DbContext.SaveChangesAsync(ct);
            });
        }

        public async Task UnassignSprintFromIssuesAsync(long sprintId, CancellationToken ct)
        {
            await _transactionUnit.ExecuteWithTransactionAsync(async () =>
            {
                var assignedIssues = await GetIssuesBySprintIdAsync(sprintId, ct);

                foreach (var issue in assignedIssues)
                {
                    issue.SprintId = null;
                    issue.UpdatedAt = DateTime.UtcNow;
                }

                await DbContext.SaveChangesAsync(ct);
            });
        }

        public Task<List<Issue>> GetIssuesBySprintIdAsync(long id, CancellationToken ct)
            => DbContext.Issues.Where(i => i.SprintId == id && !i.IsDeleted).ToListAsync(ct);

    }
}
