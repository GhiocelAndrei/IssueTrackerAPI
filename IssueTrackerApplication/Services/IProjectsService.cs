using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IProjectsService
    {
        Task<List<Project>> GetAllAsync(CancellationToken ct);
        Task<Project> GetAsync(long id, CancellationToken ct);
        Task<Project> UpdateAsync(long id, UpdateProjectCommand command, CancellationToken ct);
        Task<Project> CreateAsync(CreateProjectCommand entity, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
        Task<bool> ExistsAsync(long Id, CancellationToken ct);
    }
}
