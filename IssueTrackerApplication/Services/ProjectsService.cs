using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using AutoMapper;
using IssueTracker.DataAccess.DatabaseContext;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class ProjectsService : BaseService<Project, CreateProjectCommand, UpdateProjectCommand>, IProjectsService
    {
        private readonly IProjectRepository _projectRepository; 

        public ProjectsService(IssueContext dbContext, IMapper mapper, IProjectRepository projectRepository) : base(dbContext, mapper)
        {
            _projectRepository = projectRepository;
        }

        public Task<bool> ExistsAsync(long id, CancellationToken ct)
           => DbContext.Set<Project>().AsNoTracking().AnyAsync(u => u.Id == id, ct);

        public Task<long> GetIssueSequenceAsync(long projectId, CancellationToken ct)
           => _projectRepository.GetIssueSequenceAsync(projectId, ct);

        public async Task<string> GetProjectCodeAsync(long projectId, CancellationToken ct)
        {
            var project = await base.GetAsync(projectId, ct);
            return project.Code;
        }
    }
}
