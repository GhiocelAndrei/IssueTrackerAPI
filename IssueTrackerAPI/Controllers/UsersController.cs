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
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetAll();
            return _mapper.Map<List<UserDto>>(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(long id)
        {
            var user = await _userRepository.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserDto>(user); ;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                await _userRepository.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userRepository.Exists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var createdIssue = await _userRepository.Add(user);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _userRepository.Delete(id);

            return NoContent();
        }

    }
}
