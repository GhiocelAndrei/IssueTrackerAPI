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
        private readonly IssueService _issueService;
        private readonly IMapper _mapper;
        public IssuesController(IssueService issueService, IMapper mapper)
        {
            _issueService = issueService;
            _mapper = mapper;
        }

        // GET: api/Issues
        [HttpGet("Issues")]
        [OAuth(Scopes.IssuesRead)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues(CancellationToken cancellationToken)
        {
            var issues = await _issueService.GetAllAsync(cancellationToken);

            return _mapper.Map<List<IssueDto>>(issues);
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        [OAuth(Scopes.IssuesRead)]
        public async Task<ActionResult<IssueDto>> GetIssue(long id, CancellationToken cancellationToken)
        {
            var issue = await _issueService.GetAsync(id, cancellationToken);

            return _mapper.Map<IssueDto>(issue);
        }

        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<IActionResult> PutIssue(long id, IssueUpdatingDto issueDto, CancellationToken cancellationToken)
        {
            var issueCommand = _mapper.Map<UpdateIssueCommand>(issueDto);

            await _issueService.UpdateAsync(id, issueCommand, cancellationToken);

            return NoContent();
        }

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<ActionResult<Issue>> PostIssue(IssueCreatingDto issueDto, CancellationToken cancellationToken)
        {
            var issueCommand = _mapper.Map<CreateIssueCommand>(issueDto);

            var createdIssue = await _issueService.CreateAsync(issueCommand, cancellationToken);

            return CreatedAtAction("GetIssue", new { id = createdIssue.Id }, createdIssue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        [OAuth(Scopes.IssuesWrite)]
        public async Task<IActionResult> DeleteIssue(long id, CancellationToken cancellationToken)
        {
            await _issueService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
