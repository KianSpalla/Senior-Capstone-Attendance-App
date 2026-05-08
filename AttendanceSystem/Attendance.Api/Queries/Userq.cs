using Attendance.Api.Data;
using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Queries;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByEnumAsync(string enumValue);
    Task<User?> GetByEmailAsync(string email);
    Task<string> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(string enumValue);
}

public class UserRepository : IUserRepository
{
    private readonly AttendanceDbContext _db;

    public UserRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByEnumAsync(string enumValue)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Enum == enumValue);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.email == email);
    }

    public async Task<string> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user.Enum;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await _db.Users.FindAsync(user.Enum);

        if (existingUser is null)
            return false;

        existingUser.fname = user.fname;
        existingUser.lname = user.lname;
        existingUser.email = user.email;
        if (!string.IsNullOrWhiteSpace(user.password))
        {
            existingUser.password = user.password;
        }

        existingUser.phoneNum = user.phoneNum;
        existingUser.Role = user.Role;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string enumValue)
    {
        var user = await _db.Users.FindAsync(enumValue);

        if (user is null)
            return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
