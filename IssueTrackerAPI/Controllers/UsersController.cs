using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Application.Authorization;
using AutoMapper;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        
        [HttpGet("All")]
        [OAuth(Permissions.UsersRead)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken ct)
        {
            var users = await _userService.GetAllAsync(ct);

            return _mapper.Map<List<UserDto>>(users);
        }

        [HttpGet("{id}")]
        [OAuth(Permissions.UsersRead)]
        public async Task<ActionResult<UserDto>> GetUser(long id, CancellationToken ct)
        {
            var user = await _userService.GetAsync(id, ct);

            return _mapper.Map<UserDto>(user);
        }

        [HttpPatch("{id}")]
        [OAuth(Permissions.UsersWrite)]
        public async Task<UserDto> PatchUser(long id, JsonPatchDocument<UserUpdatingDto> userPatch, CancellationToken ct)
        {
            var user = await _userService.PatchAsync(id, userPatch, ct);

            return _mapper.Map<UserDto>(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreatingDto userDto, CancellationToken ct)
        {
            var userCommand = _mapper.Map<CreateUserCommand>(userDto);

            var createdUser = await _userService.CreateAsync(userCommand, ct);

            return CreatedAtAction("GetUser", new { id = createdUser.Id }, createdUser);
        }

        [HttpDelete("{id}")]
        [OAuth(Permissions.UsersWrite)]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken ct)
        {
            await _userService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
