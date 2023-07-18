using Microsoft.AspNetCore.Mvc;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using AutoMapper;

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
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues()
        {
            var issues = await _issueService.GetIssues();

            return _mapper.Map<List<IssueDto>>(issues);
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IssueDto>> GetIssue(long id)
        {
            var issue = await _issueService.GetIssue(id);

            if (issue == null)
            {
                return NotFound();
            }

            return _mapper.Map<IssueDto>(issue); 
        }
        
        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIssue(long id, IssueCreatingDto issueDto)
        {
            var issueCommand = _mapper.Map<IssueCommandDto>(issueDto);

            var putIssue = await _issueService.PutIssue(id, issueCommand);

            if (putIssue == null)
            {
                return BadRequest("Put Issue Failed");
            }

            return NoContent();
        }

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Issue>> PostIssue(IssueCreatingDto issueDto)
        {
            var issueCommand = _mapper.Map<IssueCommandDto>(issueDto);

            var createdIssue = await _issueService.PostIssue(issueCommand);

            return CreatedAtAction("GetIssue", new { id = createdIssue.Id }, createdIssue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(long id)
        {
            await _issueService.DeleteIssue(id);

            return NoContent();
        }
    }
}
