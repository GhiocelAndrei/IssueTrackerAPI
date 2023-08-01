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
        [OAuth(Scopes.SprintsRead)]
        public async Task<ActionResult<IEnumerable<SprintDto>>> GetSprints(CancellationToken ct)
        {
            var sprints = await _sprintsService.GetAllAsync(ct);

            return _mapper.Map<List<SprintDto>>(sprints);
        }

        [HttpGet("{id}")]
        [OAuth(Scopes.SprintsRead)]
        public async Task<ActionResult<SprintDto>> GetSprints(long id, CancellationToken ct)
        {
            var sprint = await _sprintsService.GetAsync(id, ct);

            return _mapper.Map<SprintDto>(sprint);
        }

        [HttpPut("{id}")]
        [OAuth(Scopes.SprintsWrite)]
        public async Task<IActionResult> PutSprint(long id, SprintUpdatingDto sprintDto, CancellationToken ct)
        {
            var sprintCommand = _mapper.Map<UpdateSprintCommand>(sprintDto);

            await _sprintsService.UpdateAsync(id, sprintCommand, ct);

            return NoContent();
        }

        [HttpPost]
        [OAuth(Scopes.SprintsWrite)]
        public async Task<ActionResult<Sprint>> PostSprint(SprintCreatingDto sprintDto, CancellationToken ct)
        {
            var sprintCommand = _mapper.Map<CreateSprintCommand>(sprintDto);

            var createdSprint = await _sprintsService.CreateAsync(sprintCommand, ct);

            return CreatedAtAction("GetSprint", new { id = createdSprint.Id }, createdSprint);
        }

        [HttpPost("CreateWithIssues")]
        [OAuth(Scopes.SprintsWrite, Scopes.IssuesWrite)]
        public async Task<IActionResult> PostSprintWithIssues(SprintCreatingWithIssuesDto sprintDto, CancellationToken ct)
        {
            var sprintCommand = _mapper.Map<CreateSprintWithIssuesCommand>(sprintDto);

            await _sprintsService.CreateSprintWithIssuesAsync(sprintCommand, ct);

            return Ok();
        }

        [HttpDelete("{id}")]
        [OAuth(Scopes.SprintsWrite, Scopes.IssuesWrite)]
        public async Task<IActionResult> DeleteSprint(long id, CancellationToken ct)
        {
            await _sprintsService.DeleteAsync(id, ct);

            return NoContent();
        }

        [HttpPost("{id}/close")]
        [OAuth(Scopes.SprintsWrite)]
        public async Task<IActionResult> CloseSprint(long id, CancellationToken ct)
        {
            await _sprintsService.CloseSprint(id, ct);

            return Ok();
        }
    }
}
