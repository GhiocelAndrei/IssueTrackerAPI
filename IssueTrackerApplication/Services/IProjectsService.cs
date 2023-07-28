using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IProjectsService : IBaseService<Project, CreateProjectCommand, UpdateProjectCommand>
    {
        Task<bool> ExistsAsync(long id, CancellationToken ct);
    }
}
