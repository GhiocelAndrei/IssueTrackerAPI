using Microsoft.AspNetCore.Mvc;
using IssueTracker.Abstractions.Models;
using IssueTracker.Abstractions.Mapping;
using IssueTracker.Application.Services;
using IssueTracker.Abstractions.Definitions;
using IssueTracker.Application.Authorization;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace IssueTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly AuthorizationService _authorizationService;

        public UsersController(IUsersService userService, IMapper mapper, 
            IOptions<AppSettings> appSettings, AuthorizationService authorizationService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _authorizationService = authorizationService;
        }
        
        [HttpGet("All")]
        [OAuth(Scopes.UsersRead)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken ct)
        {
            var users = await _userService.GetAllAsync(ct);

            return _mapper.Map<List<UserDto>>(users);
        }

        [HttpGet("{id}")]
        [OAuth(Scopes.UsersRead)]
        public async Task<ActionResult<UserDto>> GetUser(long id, CancellationToken ct)
        {
            var user = await _userService.GetAsync(id, ct);

            return _mapper.Map<UserDto>(user);
        }

        [HttpPut("{id}")]
        [OAuth(Scopes.UsersWrite)]
        public async Task<IActionResult> PutUser(long id, UserUpdatingDto userDto, CancellationToken ct)
        {
            var userCommand = _mapper.Map<UpdateUserCommand>(userDto);

            await _userService.UpdateAsync(id, userCommand, ct);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreatingDto userDto, CancellationToken ct)
        {
            var userCommand = _mapper.Map<CreateUserCommand>(userDto);

            var createdUser = await _userService.CreateAsync(userCommand, ct);

            return CreatedAtAction("GetUser", new { id = createdUser.Id }, createdUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(UserLoginDto userDto, CancellationToken ct)
        {
            var userCommand = _mapper.Map<LoginUserCommand>(userDto);

            var role = await _userService.LoginUserAsync(userCommand, ct);

            string token = _authorizationService.CreateToken(role, _appSettings.Secret);

            return Ok(token);
        }

        [HttpDelete("{id}")]
        [OAuth(Scopes.UsersWrite)]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken ct)
        {
            await _userService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
