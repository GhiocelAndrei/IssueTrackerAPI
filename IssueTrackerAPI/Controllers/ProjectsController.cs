using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IssueTrackerAPI.DatabaseContext;
using IssueTrackerAPI.Models;
using AutoMapper;
using IssueTrackerAPI.Mapping;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public ProjectsController(IRepository<Project> issueRepository, IMapper mapper)
        {
            _projectRepository = issueRepository;
            _mapper = mapper;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProject()
        {
            var projects = await _projectRepository.GetAll();
            return _mapper.Map<List<ProjectDto>>(projects);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(long id)
        {
            var project = await _projectRepository.Get(id);

            if (project == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProjectDto>(project);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(long id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            try
            {
                await _projectRepository.Update(project);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _projectRepository.Exists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            var createdProject = await _projectRepository.Add(project);

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            await _projectRepository.Delete(id);

            return NoContent();
        }

    }
}
