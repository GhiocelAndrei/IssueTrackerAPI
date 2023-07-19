using Microsoft.AspNetCore.Mvc;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Application.Services;
using AutoMapper;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectsController(ProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _projectService.GetAll();

            return _mapper.Map<List<ProjectDto>>(projects);
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(long id)
        {
            var project = await _projectService.Get(id);

            if (project == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProjectDto>(project);
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(long id, ProjectUpdatingDto projectDto)
        {
            var projectCommand = _mapper.Map<UpdateProjectCommand>(projectDto);

            var putProject = await _projectService.Update(id, projectCommand);

            if (putProject == null)
            {
                return BadRequest("Put Issue Failed");
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectCreatingDto projectDto)
        {
            var projectCommand = _mapper.Map<CreateProjectCommand>(projectDto);

            var createdProject = await _projectService.Create(projectCommand);

            return CreatedAtAction("GetProject", new { id = createdProject.Id }, createdProject);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            await _projectService.Delete(id);

            return NoContent();
        }

    }
}
