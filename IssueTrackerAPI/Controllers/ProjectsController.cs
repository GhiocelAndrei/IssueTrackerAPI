using Microsoft.AspNetCore.Mvc;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Application.Services;
using IssueTracker.Application.Authorization;
using AutoMapper;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectService;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectsService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [OAuth(Scopes.ProjectsRead)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(CancellationToken ct)
        {
            var projects = await _projectService.GetAllAsync(ct);

            return _mapper.Map<List<ProjectDto>>(projects);
        }

        [HttpGet("{id}")]
        [OAuth(Scopes.ProjectsRead)]
        public async Task<ActionResult<ProjectDto>> GetProject(long id, CancellationToken ct)
        {
            var project = await _projectService.GetAsync(id, ct);

            return _mapper.Map<ProjectDto>(project);
        }

        [HttpPut("{id}")]
        [OAuth(Scopes.ProjectsWrite)]
        public async Task<IActionResult> PutProject(long id, ProjectUpdatingDto projectDto, CancellationToken ct)
        {
            var projectCommand = _mapper.Map<UpdateProjectCommand>(projectDto);

            await _projectService.UpdateAsync(id, projectCommand, ct);

            return NoContent();
        }

        [HttpPost]
        [OAuth(Scopes.ProjectsWrite)]
        public async Task<ActionResult<Project>> PostProject(ProjectCreatingDto projectDto, CancellationToken ct)
        {
            var projectCommand = _mapper.Map<CreateProjectCommand>(projectDto);

            var createdProject = await _projectService.CreateAsync(projectCommand, ct);

            return CreatedAtAction("GetProject", new { id = createdProject.Id }, createdProject);
        }

        [HttpDelete("{id}")]
        [OAuth(Scopes.ProjectsWrite)]
        public async Task<IActionResult> DeleteProject(long id, CancellationToken ct)
        {
            await _projectService.DeleteAsync(id, ct);

            return NoContent();
        }

    }
}
