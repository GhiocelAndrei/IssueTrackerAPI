using AutoMapper;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.DataAccess.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTracker.Application.Services
{
    public class IssueService : BaseService<Issue, CreateIssueCommand, UpdateIssueCommand>
    {
        protected readonly UserService _userService;
        public IssueService(IssueContext dbContext, IMapper mapper, UserService userService) : base(dbContext, mapper)
        {
            _userService = userService;
        }

        public override async Task<Issue> CreateAsync(CreateIssueCommand entity, CancellationToken ct)
        {
            var reporterExists = await _userService.ExistsAsync(entity.ReporterId, ct);

            if (!reporterExists)
            {
                throw new InvalidInputException("Given input is invalid");
            }

            var result = await base.CreateAsync(entity, ct);

            return result;
        }
    }
}
