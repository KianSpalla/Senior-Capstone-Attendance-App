using Attendance.Api.Models;
using Attendance.Api.Queries;
using System.Security.Cryptography;

namespace Attendance.Api.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByEnumAsync(string enumValue);
    Task<User?> LoginAsync(string email, string password);
    Task<string> CreateUserAsync(User user, string? password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string enumValue);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<User?> GetUserByEnumAsync(string enumValue)
    {
        return await _repo.GetByEnumAsync(enumValue);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _repo.GetByEmailAsync(email.Trim().ToLowerInvariant());

        if (user is null)
            return null;

        if (user.Role is UserRole.student)
            return null;

        return VerifyPassword(password, user.password) ? user : null;
    }

    public async Task<string> CreateUserAsync(User user, string? password)
    {
        user.Enum = user.Enum.Trim().ToLowerInvariant();
        user.email = user.email.Trim().ToLowerInvariant();
        user.password = user.Role is UserRole.student
            ? "NO_WEB_LOGIN"
            : HashPassword(password ?? throw new ArgumentException("Password is required for admin and teacher users."));
        user.createdAt ??= DateTime.UtcNow;

        return await _repo.CreateAsync(user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        user.Enum = user.Enum.Trim().ToLowerInvariant();
        user.email = user.email.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(user.password) && !IsHashedPassword(user.password))
        {
            user.password = HashPassword(user.password);
        }

        return await _repo.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(string enumValue)
    {
        return await _repo.DeleteAsync(enumValue);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return $"PBKDF2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedPassword)
    {
        if (!IsHashedPassword(storedPassword))
        {
            return password == storedPassword;
        }

        var parts = storedPassword.Split('$');
        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = Convert.FromBase64String(parts[2]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static bool IsHashedPassword(string password)
    {
        return password.StartsWith("PBKDF2$", StringComparison.Ordinal)
            && password.Split('$').Length == 3;
    }
}
