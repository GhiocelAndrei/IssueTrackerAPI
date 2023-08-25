using AutoMapper;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Models;
using IssueTracker.Application.Authorization;
using IssueTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

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
        [OAuth(Permissions.IssuesRead)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues(CancellationToken ct)
        {
            var issues = await _issueService.GetAllAsync(ct);

            return _mapper.Map<List<IssueDto>>(issues);
        }

        [HttpGet("{id}")]
        [OAuth(Permissions.IssuesRead)]
        public async Task<ActionResult<IssueDto>> GetIssue(long id, CancellationToken ct)
        {
            var issue = await _issueService.GetAsync(id, ct);

            return _mapper.Map<IssueDto>(issue);
        }

        [HttpGet("Sprint/{id}")]
        [OAuth(Permissions.IssuesRead)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssuesBySprintId(long id, CancellationToken ct)
        {
            var issues = await _issueService.GetIssuesBySprintIdAsync(id, ct);

            return _mapper.Map<List<IssueDto>>(issues);
        }

        [HttpPatch("{id}")]
        [OAuth(Permissions.IssuesWrite)]
        public async Task<IssueDto> PatchIssue(long id, JsonPatchDocument<IssueUpdatingDto> issuePatch, CancellationToken ct)
        {
            var issue = await _issueService.PatchAsync(id, issuePatch, ct);

            return _mapper.Map<IssueDto>(issue);
        }

        [HttpPut("AssignSprint")]
        [OAuth(Permissions.IssuesWrite)]
        public async Task<IActionResult> AssignSprintToIssues(List<long> ids, long sprintId, CancellationToken ct)
        {
            await _issueService.AssignSprintToIssuesAsync(ids, sprintId, ct);

            return NoContent();
        }

        [HttpPost]
        [OAuth(Permissions.IssuesWrite)]
        public async Task<ActionResult<Issue>> PostIssue(IssueCreatingDto issueDto, CancellationToken ct)
        {
            var issueCommand = _mapper.Map<CreateIssueCommand>(issueDto);

            var createdIssue = await _issueService.CreateAsync(issueCommand, ct);

            return CreatedAtAction("GetIssue", new { id = createdIssue.Id }, createdIssue);
        }

        [HttpDelete("{id}")]
        [OAuth(Permissions.IssuesWrite)]
        public async Task<IActionResult> DeleteIssue(long id, CancellationToken ct)
        {
            await _issueService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
