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
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly AuthorizationService _authorizationService;

        public UsersController(UserService userService, IMapper mapper, 
            IOptions<AppSettings> appSettings, AuthorizationService authorizationService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _authorizationService = authorizationService;
        }
        
        // GET: api/Users
        [HttpGet]
        [OAuth(Scopes.UsersRead)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAll();

            return _mapper.Map<List<UserDto>>(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [OAuth(Scopes.UsersRead)]
        public async Task<ActionResult<UserDto>> GetUser(long id)
        {
            var user = await _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserDto>(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [OAuth(Scopes.UsersWrite)]
        public async Task<IActionResult> PutUser(long id, UserUpdatingDto userDto)
        {
            var userCommand = _mapper.Map<UpdateUserCommand>(userDto);

            var putUser = await _userService.Update(id, userCommand);

            if (putUser == null)
            {
                return BadRequest("Put Issue Failed");
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreatingDto userDto)
        {
            var userCommand = _mapper.Map<CreateUserCommand>(userDto);

            var createdUser = await _userService.Create(userCommand);

            return CreatedAtAction("GetUser", new { id = createdUser.Id }, createdUser);
        }

        // Login
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(UserLoginDto userDto)
        {
            var userCommand = _mapper.Map<LoginUserCommand>(userDto);

            var role = await _userService.LoginUserAsync(userCommand);

            if (role == null)
            {
                return NotFound("User not found.");
            }

            string token = _authorizationService.CreateToken(role, _appSettings.Secret);

            return Ok(token);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [OAuth(Scopes.UsersWrite)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _userService.Delete(id);

            return NoContent();
        }
    }
}
