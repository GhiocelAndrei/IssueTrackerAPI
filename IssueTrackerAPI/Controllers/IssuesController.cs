using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackerAPI.DatabaseContext;
using IssueTrackerAPI.Models;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly IssueContext _context;

        public IssuesController(IssueContext context)
        {
            _context = context;
        }

        // GET: api/Issues
        [HttpGet("Issues")]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
        {
          if (_context.Issues == null)
          {
              return NotFound();
          }
            return await _context.Issues.ToListAsync();
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Issue>> GetIssue(long id)
        {
          if (_context.Issues == null)
          {
              return NotFound();
          }
            var issue = await _context.Issues.FindAsync(id);

            if (issue == null)
            {
                return NotFound();
            }

            return issue;
        }

        //Get: api/Issues/Asignee/{name}
        [HttpGet("Asignee/{name}")]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssuesForName(string Name)
        {
            if (_context.Issues == null)
            {
                return NotFound();
            }
            var issue = await _context.Issues
                .Where(i => i.Asignee.Name == Name)
                .ToListAsync();

            if (issue == null || !issue.Any())
            {
                return NotFound();
            }

            return issue;
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

            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
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
          if (_context.Issues == null)
          {
              return Problem("Entity set 'IssueContext.Issues'  is null.");
          }

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIssue", new { id = issue.Id }, issue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(long id)
        {
            if (_context.Issues == null)
            {
                return NotFound();
            }
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IssueExists(long id)
        {
            return (_context.Issues?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
