using AttendanceApp.Models;
using AttendanceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/user
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET api/user/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        // POST api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var user = new User
            {
                Enum      = request.Enum,
                FName     = request.FName,
                LName     = request.LName,
                Email     = request.Email,
                PhoneNum  = request.PhoneNum,
                Role      = request.Role
            };

            var newId = await _userService.CreateUserAsync(user, request.Password);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { userId = newId });
        }

        // PUT api/user/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            user.UserId = id;
            var success = await _userService.UpdateUserAsync(user);
            return success ? NoContent() : NotFound();
        }

        // DELETE api/user/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            return success ? NoContent() : NotFound();
        }
    }

    // DTO to accept plain-text password on creation
    public record CreateUserRequest(
        string? Enum,
        string FName,
        string LName,
        string Email,
        string Password,
        string? PhoneNum,
        UserRole Role
    );
}