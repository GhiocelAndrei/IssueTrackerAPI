using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;

namespace IssueTracker.Application.Services
{
    public interface IIssuesService : IBaseService<Issue, CreateIssueCommand, UpdateIssueCommand>
    {
    }
}
