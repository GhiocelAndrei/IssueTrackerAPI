using AutoMapper;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public abstract class BaseService<T, TCreateCommand, TUpdateCommand> 
        where T : class
        where TCreateCommand : class
        where TUpdateCommand : class
    {
        protected readonly IGenericRepository<T> _repository;
        protected readonly IMapper _mapper;

        public BaseService(IGenericRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var issues = await _repository.GetAllAsync();

            return issues;
        }

        public async Task<T> Get(long id)
        {
            var issue = await _repository.GetAsync(id);

            return issue;
        }

        public async Task<T> Update(long id, TUpdateCommand command)
        {
            var issueModified = await _repository.GetAsync(id);

            if (issueModified == null)
            {
                return null;
            }

            _mapper.Map(command, issueModified);

            await _repository.UpdateAsync(issueModified);
            
            return issueModified;
        }

        public async Task<T> Create(TCreateCommand command)
        {
            var issue = _mapper.Map<T>(command);

            var createdIssue = await _repository.AddAsync(issue);

            return createdIssue;
        }

        public async Task Delete(long id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
