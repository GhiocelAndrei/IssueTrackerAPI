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
    public class SprintsController : ControllerBase
    {
        private readonly ISprintsService _sprintsService;
        private readonly IMapper _mapper;

        public SprintsController(ISprintsService sprintsService, IMapper mapper)
        {
            _sprintsService = sprintsService;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [OAuth(Permissions.SprintsRead)]
        public async Task<ActionResult<IEnumerable<SprintDto>>> GetSprints(CancellationToken ct)
        {
            var sprints = await _sprintsService.GetAllAsync(ct);

            return _mapper.Map<List<SprintDto>>(sprints);
        }

        [HttpGet("{id}")]
        [OAuth(Permissions.SprintsRead)]
        public async Task<ActionResult<SprintDto>> GetSprints(long id, CancellationToken ct)
        {
            var sprint = await _sprintsService.GetAsync(id, ct);

            return _mapper.Map<SprintDto>(sprint);
        }

        [HttpPatch("{id}")]
        [OAuth(Permissions.SprintsWrite)]
        public async Task<SprintDto> PatchSprint(long id, JsonPatchDocument<SprintUpdatingDto> sprintPatch, CancellationToken ct)
        {
            var sprint = await _sprintsService.PatchAsync(id, sprintPatch, ct);

            return _mapper.Map<SprintDto>(sprint);
        }

        [HttpPost]
        [OAuth(Permissions.SprintsWrite)]
        public async Task<ActionResult<Sprint>> PostSprint(SprintCreatingDto sprintDto, CancellationToken ct)
        {
            var sprintCommand = _mapper.Map<CreateSprintCommand>(sprintDto);

            var createdSprint = await _sprintsService.CreateAsync(sprintCommand, ct);

            return CreatedAtAction("GetSprint", new { id = createdSprint.Id }, createdSprint);
        }

        [HttpPost("CreateWithIssues")]
        [OAuth(Permissions.SprintsWrite, Permissions.IssuesWrite)]
        public async Task<IActionResult> PostSprintWithIssues(SprintCreatingWithIssuesDto sprintDto, CancellationToken ct)
        {
            var sprintCommand = _mapper.Map<CreateSprintWithIssuesCommand>(sprintDto);

            await _sprintsService.CreateSprintWithIssuesAsync(sprintCommand, ct);

            return Ok();
        }

        [HttpDelete("{id}")]
        [OAuth(Permissions.SprintsWrite, Permissions.IssuesWrite)]
        public async Task<IActionResult> DeleteSprint(long id, CancellationToken ct)
        {
            await _sprintsService.DeleteAsync(id, ct);

            return NoContent();
        }

        [HttpPost("{id}/close")]
        [OAuth(Permissions.SprintsWrite)]
        public async Task<IActionResult> CloseSprint(long id, CancellationToken ct)
        {
            await _sprintsService.CloseSprint(id, ct);

            return Ok();
        }
    }
}
