using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.Abstractions.Exceptions;

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
            var projectExists = await _projectService.ExistsAsync(entity.ProjectId, ct);

            if (!projectExists)
            {
                throw new InvalidInputException("Project ID does not exits");
            }

            var reporterExists = await _userService.ExistsAsync(entity.ReporterId, ct);

            if (!reporterExists)
            {
                throw new InvalidInputException("Reporter ID does not exits");
            }

            var assigneeExists = await _userService.ExistsAsync(entity.AssigneeId, ct);

            if (!assigneeExists)
            {
                throw new InvalidInputException("Assignee ID does not exits");
            }

            return await base.CreateAsync(entity, ct);
        }
    }
}
