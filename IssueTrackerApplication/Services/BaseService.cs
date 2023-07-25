using AutoMapper;
using IssueTracker.DataAccess.Repositories;
using IssueTracker.Abstractions.Exceptions;

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

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            var issues = await _repository.GetAllAsync(cancellationToken);

            return issues;
        }

        public async Task<T> GetAsync(long id, CancellationToken cancellationToken)
        {
            var issue = await _repository.GetAsync(id, cancellationToken);

            if(issue == null) 
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            return issue;
        }
        
        public async Task<T> UpdateAsync(long id, TUpdateCommand command, CancellationToken cancellationToken)
        {
            var issueModified = await _repository.GetAsync(id, cancellationToken);

            if (issueModified == null)
            {
                throw new NotFoundException("The requested entity could not be found.");
            }

            _mapper.Map(command, issueModified);

            await _repository.UpdateAsync(issueModified, cancellationToken);
            
            return issueModified;
        }

        public async Task<T> CreateAsync(TCreateCommand command, CancellationToken cancellationToken)
        {
            var issue = _mapper.Map<T>(command);

            var createdIssue = await _repository.AddAsync(issue, cancellationToken);

            return createdIssue;
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
