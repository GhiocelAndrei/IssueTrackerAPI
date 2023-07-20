using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.DataAccess.Repositories;
using AutoMapper;

namespace IssueTracker.Application.Services
{
    public class ProjectService : BaseService<Project, CreateProjectCommand, UpdateProjectCommand>
    {
        public ProjectService(IGenericRepository<Project> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
