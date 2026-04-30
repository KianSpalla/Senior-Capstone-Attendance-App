using Attendance.Api.Models;
using Attendance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users.Select(ToResponse));
        }

        [HttpGet("{enumValue}")]
        public async Task<IActionResult> GetByEnum(string enumValue)
        {
            var user = await _userService.GetUserByEnumAsync(enumValue);
            return user is null ? NotFound() : Ok(ToResponse(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.email, request.password);

            return user is null
                ? Unauthorized("Invalid email or password.")
                : Ok(ToResponse(user));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var user = new User
            {
                Enum = request.Enum,
                fname = request.fname,
                lname = request.lname,
                email = request.email,
                phoneNum = request.phoneNum,
                Role = request.Role
            };

            var newEnum = await _userService.CreateUserAsync(user, request.password);

            return CreatedAtAction(
                nameof(GetByEnum),
                new { enumValue = newEnum },
                new { Enum = newEnum }
            );
        }

        [HttpPut("{enumValue}")]
        public async Task<IActionResult> Update(string enumValue, [FromBody] User user)
        {
            user.Enum = enumValue;

            var success = await _userService.UpdateUserAsync(user);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{enumValue}")]
        public async Task<IActionResult> Delete(string enumValue)
        {
            var success = await _userService.DeleteUserAsync(enumValue);
            return success ? NoContent() : NotFound();
        }

        private static UserResponse ToResponse(User user)
        {
            return new UserResponse(
                user.Enum,
                user.fname,
                user.lname,
                user.email,
                user.phoneNum,
                user.Role,
                user.createdAt
            );
        }
    }

    public record LoginRequest(string email, string password);

    public record CreateUserRequest(
        string Enum,
        string fname,
        string lname,
        string email,
        string? password,
        string? phoneNum,
        UserRole Role
    );

    public record UserResponse(
        string Enum,
        string fname,
        string lname,
        string email,
        string? phoneNum,
        UserRole Role,
        DateTime? createdAt
    );
}
