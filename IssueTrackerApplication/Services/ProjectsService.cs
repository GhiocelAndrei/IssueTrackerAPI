using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using AutoMapper;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class ProjectsService : BaseService<Project, CreateProjectCommand, UpdateProjectCommand>, IProjectsService
    {
        public ProjectsService(IssueContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<bool> ExistsAsync(long id, CancellationToken ct)
           => DbContext.Set<Project>().AsNoTracking().AnyAsync(u => u.Id == id, ct);
    }
}
