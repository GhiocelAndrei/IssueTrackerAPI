using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Services
{
    public class IssueService
    {
        private readonly IGenericRepository<Issue> _issueRepository;
        private readonly IMapper _mapper;

        public IssueService(IGenericRepository<Issue> issueRepository, IMapper mapper)
        {
            _issueRepository = issueRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Issue>> GetIssues()
        {
            var issues = await _issueRepository.GetAll();

            return issues;
        }

        public async Task<Issue> GetIssue(long id)
        {
            var issue = await _issueRepository.Get(id);

            return issue;
        }

        public async Task<Issue> PutIssue(long id, IssueCommandDto issueCommand)
        {
            var issueModified = await _issueRepository.Get(id);

            if (issueModified == null)
            {
                return null;
            }

            _mapper.Map(issueCommand, issueModified);

            try
            {
                await _issueRepository.Update(issueModified);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (issueModified == null)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return issueModified;
        }

        public async Task<Issue> PostIssue(IssueCommandDto issueCommand)
        {
            var issue = _mapper.Map<Issue>(issueCommand);

            var createdIssue = await _issueRepository.Add(issue);

            return createdIssue;
        }

        public async Task DeleteIssue(long id)
        {
            await _issueRepository.Delete(id);
        }
    }
}
