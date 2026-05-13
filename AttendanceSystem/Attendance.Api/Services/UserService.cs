using Attendance.Api.Models;
using Attendance.Api.Queries;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Attendance.Api.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByEnumAsync(string enumValue);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> LoginByEnumAsync(string enumValue, string password);
    Task<WebLoginStatus?> GetWebLoginStatusAsync(string enumValue);
    Task<User?> SetWebPasswordAsync(string enumValue, string password);
    Task<string> CreateUserAsync(User user, string? password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string enumValue);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private static readonly Regex EnumFormat = new("^e[0-9]{7}$", RegexOptions.Compiled);
    private static readonly Regex UppercaseLetter = new("[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercaseLetter = new("[a-z]", RegexOptions.Compiled);
    private static readonly Regex Digit = new("[0-9]", RegexOptions.Compiled);
    private static readonly Regex Symbol = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

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

    public async Task<User?> LoginByEnumAsync(string enumValue, string password)
    {
        enumValue = enumValue.Trim().ToLowerInvariant();
        ValidateEnum(enumValue);

        var user = await _repo.GetByEnumAsync(enumValue);

        if (user is null)
            return null;

        if (user.Role is UserRole.student)
            return null;

        if (NeedsPasswordSetup(user.password))
            return null;

        return VerifyPassword(password, user.password) ? user : null;
    }

    public async Task<WebLoginStatus?> GetWebLoginStatusAsync(string enumValue)
    {
        enumValue = enumValue.Trim().ToLowerInvariant();
        ValidateEnum(enumValue);

        var user = await _repo.GetByEnumAsync(enumValue);

        if (user is null)
            return null;

        return new WebLoginStatus(
            user.Enum,
            user.fname,
            user.lname,
            user.Role,
            user.Role is UserRole.admin or UserRole.teacher,
            NeedsPasswordSetup(user.password)
        );
    }

    public async Task<User?> SetWebPasswordAsync(string enumValue, string password)
    {
        enumValue = enumValue.Trim().ToLowerInvariant();
        ValidateEnum(enumValue);
        ValidatePassword(password);

        var user = await _repo.GetByEnumAsync(enumValue);

        if (user is null)
            return null;

        if (user.Role is UserRole.student)
            throw new ArgumentException("Only admin or teacher users can create a web app password.");

        user.password = HashPassword(password);
        return await _repo.UpdateAsync(user) ? user : null;
    }

    public async Task<string> CreateUserAsync(User user, string? password)
    {
        user.Enum = user.Enum.Trim().ToLowerInvariant();
        ValidateEnum(user.Enum);
        user.email = user.email.Trim().ToLowerInvariant();
        user.password = user.Role is UserRole.student
            ? "NO_WEB_LOGIN"
            : HashPassword(ValidatePassword(password ?? throw new ArgumentException("Password is required for admin and teacher users.")));
        user.createdAt ??= DateTime.UtcNow;

        return await _repo.CreateAsync(user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        user.Enum = user.Enum.Trim().ToLowerInvariant();
        ValidateEnum(user.Enum);
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

    private static bool NeedsPasswordSetup(string password)
    {
        return string.Equals(password, "NO_WEB_LOGIN", StringComparison.Ordinal);
    }

    private static void ValidateEnum(string enumValue)
    {
        if (!EnumFormat.IsMatch(enumValue))
        {
            throw new ArgumentException("Enum must start with e and be followed by 7 digits.");
        }
    }

    private static string ValidatePassword(string password)
    {
        if (password.Length < 12
            || !UppercaseLetter.IsMatch(password)
            || !LowercaseLetter.IsMatch(password)
            || !Digit.IsMatch(password)
            || !Symbol.IsMatch(password))
        {
            throw new ArgumentException("Password must be at least 12 characters and include uppercase, lowercase, number, and symbol.");
        }

        return password;
    }
}

public record WebLoginStatus(
    string Enum,
    string fname,
    string lname,
    UserRole Role,
    bool CanUseWebApp,
    bool RequiresPasswordSetup
);
