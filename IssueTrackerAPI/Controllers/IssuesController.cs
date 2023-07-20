﻿using Microsoft.AspNetCore.Mvc;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "IssuesAccess")]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues()
        {
            var issues = await _issueService.GetAll();

            return _mapper.Map<List<IssueDto>>(issues);
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        [Authorize(Policy = "IssuesAccess")]
        public async Task<ActionResult<IssueDto>> GetIssue(long id)
        {
            var issue = await _issueService.Get(id);

            if (issue == null)
            {
                return NotFound();
            }

            return _mapper.Map<IssueDto>(issue); 
        }
        
        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "IssuesAccess")]
        public async Task<IActionResult> PutIssue(long id, IssueUpdatingDto issueDto)
        {
            var issueCommand = _mapper.Map<UpdateIssueCommand>(issueDto);

            var putIssue = await _issueService.Update(id, issueCommand);

            if (putIssue == null)
            {
                return BadRequest("Put Issue Failed");
            }

            return NoContent();
        }

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "IssuesAccess")]
        public async Task<ActionResult<Issue>> PostIssue(IssueCreatingDto issueDto)
        {
            var issueCommand = _mapper.Map<CreateIssueCommand>(issueDto);

            var createdIssue = await _issueService.Create(issueCommand);

            return CreatedAtAction("GetIssue", new { id = createdIssue.Id }, createdIssue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "IssuesAccess")]
        public async Task<IActionResult> DeleteIssue(long id)
        {
            await _issueService.Delete(id);

            return NoContent();
        }
    }
}
