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

        [HttpPost("login/enum")]
        public async Task<IActionResult> LoginByEnum([FromBody] EnumLoginRequest request)
        {
            try
            {
                var user = await _userService.LoginByEnumAsync(request.Enum, request.password);

                return user is null
                    ? Unauthorized("Invalid enum or password.")
                    : Ok(ToResponse(user));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{enumValue}/web-login-status")]
        public async Task<IActionResult> GetWebLoginStatus(string enumValue)
        {
            try
            {
                var status = await _userService.GetWebLoginStatusAsync(enumValue);
                return status is null ? NotFound("User not found.") : Ok(status);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{enumValue}/password")]
        public async Task<IActionResult> SetPassword(string enumValue, [FromBody] PasswordSetupRequest request)
        {
            try
            {
                var user = await _userService.SetWebPasswordAsync(enumValue, request.password);
                return user is null ? NotFound("User not found.") : Ok(ToResponse(user));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

            string newEnum;
            try
            {
                newEnum = await _userService.CreateUserAsync(user, request.password);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

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

            bool success;
            try
            {
                success = await _userService.UpdateUserAsync(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

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

    public record EnumLoginRequest(string Enum, string password);

    public record PasswordSetupRequest(string password);

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
