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

        public Task<bool> ExistsAsync(long Id, CancellationToken ct)
           => DbContext.Set<User>().AsNoTracking().AnyAsync(u => u.Id == Id, ct);
    }
}
