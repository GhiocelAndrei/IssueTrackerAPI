using AutoMapper;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Application.Authorization;
using IssueTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly IIssuesService _issueService;
        private readonly IMapper _mapper;
        public IssuesController(IIssuesService issueService, IMapper mapper)
        {
            _issueService = issueService;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [OAuth(Scopes.IssuesRead)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues(CancellationToken ct)
        {
            var issues = await _issueService.GetAllAsync(ct);

            return _mapper.Map<List<IssueDto>>(issues);
        }

        [HttpGet("{id}")]
        [OAuth(Scopes.IssuesRead)]
        public async Task<ActionResult<IssueDto>> GetIssue(long id, CancellationToken ct)
        {
            var issue = await _issueService.GetAsync(id, ct);

            return _mapper.Map<IssueDto>(issue);
        }

        [HttpGet("Sprint/{id}")]
        [OAuth(Scopes.IssuesRead)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssuesBySprintId(long id, CancellationToken ct)
        {
            var issues = await _issueService.GetIssuesBySprintIdAsync(id, ct);

            return _mapper.Map<List<IssueDto>>(issues);
        }

        [HttpPut("{id}")]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<IActionResult> PutIssue(long id, IssueUpdatingDto issueDto, CancellationToken ct)
        {
            var issueCommand = _mapper.Map<UpdateIssueCommand>(issueDto);

            await _issueService.UpdateAsync(id, issueCommand, ct);

            return NoContent();
        }

        [HttpPut("AssignSprint")]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<IActionResult> AssignSprintToIssues(List<long> ids, long sprintId, CancellationToken ct)
        {
            await _issueService.AssignSprintToIssuesAsync(ids, sprintId, ct);

            return NoContent();
        }

        [HttpPost]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<ActionResult<Issue>> PostIssue(IssueCreatingDto issueDto, CancellationToken ct)
        {
            var issueCommand = _mapper.Map<CreateIssueCommand>(issueDto);

            var createdIssue = await _issueService.CreateAsync(issueCommand, ct);

            return CreatedAtAction("GetIssue", new { id = createdIssue.Id }, createdIssue);
        }

        [HttpDelete("{id}")]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<IActionResult> DeleteIssue(long id, CancellationToken ct)
        {
            await _issueService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
