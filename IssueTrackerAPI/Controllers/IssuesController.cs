using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackerAPI.DatabaseContext;
using IssueTrackerAPI.Models;
using IssueTrackerAPI.Mapping;
using AutoMapper;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly IRepository<Issue> _issueRepository;
        private readonly IMapper _mapper;
        public IssuesController(IRepository<Issue> issueRepository, IMapper mapper)
        {
            _issueRepository = issueRepository;
            _mapper = mapper;
        }

        // GET: api/Issues
        [HttpGet("Issues")]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssues()
        {
            var issues = await _issueRepository.GetAll();
            return _mapper.Map<List<IssueDto>>(issues);
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IssueDto>> GetIssue(long id)
        {
            var issue = await _issueRepository.Get(id);

            if (issue == null)
            {
                return NotFound();
            }

            return _mapper.Map<IssueDto>(issue); 
        }

        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIssue(long id, Issue issue)
        {
            if (id != issue.Id)
            {
                return BadRequest();
            }

            try
            {
                await _issueRepository.Update(issue);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _issueRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Issue>> PostIssue(Issue issue)
        {
            var createdIssue = await _issueRepository.Add(issue);

            return CreatedAtAction("GetIssue", new { id = issue.Id }, issue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(long id)
        {
            await _issueRepository.Delete(id);

            return NoContent();
        }
    }
}
