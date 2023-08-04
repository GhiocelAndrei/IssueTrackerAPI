using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.Abstractions.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace IssueTracker.Application.Services
{
    public class IssuesService : BaseService<Issue, CreateIssueCommand, UpdateIssueCommand>, IIssuesService
    {
        private readonly IUsersService _userService;
        private readonly IProjectsService _projectService;
        
        public IssuesService(IssueContext dbContext, IMapper mapper, IUsersService userService, IProjectsService projectService)
            : base(dbContext, mapper)
        {
            _userService = userService;
            _projectService = projectService;
        }

        public override async Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken ct)
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
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

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
            transactionScope.Complete();
        }

        public async Task UnassignSprintFromIssuesAsync(long sprintId, CancellationToken ct)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var assignedIssues = await GetIssuesBySprintIdAsync(sprintId, ct);

            foreach (var issue in assignedIssues)
            {
                issue.SprintId = null;
                issue.UpdatedAt = DateTime.UtcNow;
            }

            await DbContext.SaveChangesAsync(ct);
            transactionScope.Complete();
        }

        public Task<List<Issue>> GetIssuesBySprintIdAsync(long id, CancellationToken ct)
            => DbContext.Issues.Where(i => i.SprintId == id && !i.IsDeleted).ToListAsync(ct);

    }
}
