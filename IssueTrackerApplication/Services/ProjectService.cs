using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using AutoMapper;
using IssueTracker.DataAccess.DatabaseContext;

namespace IssueTracker.Application.Services
{
    public class ProjectService : BaseService<Project, CreateProjectCommand, UpdateProjectCommand>
    {
        public ProjectService(IssueContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}
