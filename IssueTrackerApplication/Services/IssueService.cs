using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.Repositories;

namespace IssueTracker.Application.Services
{
    public class IssueService : BaseService<Issue, CreateIssueCommand, UpdateIssueCommand>
    {
        public IssueService(IGenericRepository<Issue> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
