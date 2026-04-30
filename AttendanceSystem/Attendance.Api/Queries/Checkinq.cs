using Attendance.Api.Data;
using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Queries;

public interface ICheckinRepository
{
    Task<IEnumerable<Checkin>> GetByEventAsync(int eventId);
    Task<IEnumerable<Checkin>> GetByUserAsync(string enumValue);
    Task<Checkin?> GetAsync(int eventId, string enumValue);
    Task<int> GetCheckinCountAsync(int eventId);
    Task<int> CreateAsync(Checkin checkin);
    Task<bool> DeleteAsync(int checkInId);
    Task<bool> DeleteByEventAndUserAsync(int eventId, string enumValue);
}

public class CheckinRepository : ICheckinRepository
{
    private readonly AttendanceDbContext _db;

    public CheckinRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Checkin>> GetByEventAsync(int eventId)
    {
        return await _db.Checkins
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.eventId == eventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Checkin>> GetByUserAsync(string enumValue)
    {
        return await _db.Checkins
            .AsNoTracking()
            .Include(c => c.Event)
            .Where(c => c.Enum == enumValue)
            .ToListAsync();
    }

    public async Task<Checkin?> GetAsync(int eventId, string enumValue)
    {
        return await _db.Checkins
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.eventId == eventId && c.Enum == enumValue);
    }

    public async Task<int> GetCheckinCountAsync(int eventId)
    {
        return await _db.Checkins
            .CountAsync(c => c.eventId == eventId);
    }

    public async Task<int> CreateAsync(Checkin checkin)
    {
        _db.Checkins.Add(checkin);
        await _db.SaveChangesAsync();
        return checkin.checkInId;
    }

    public async Task<bool> DeleteAsync(int checkInId)
    {
        var checkin = await _db.Checkins.FindAsync(checkInId);

        if (checkin is null)
            return false;

        _db.Checkins.Remove(checkin);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByEventAndUserAsync(int eventId, string enumValue)
    {
        var checkin = await GetAsync(eventId, enumValue);

        if (checkin is null)
            return false;

        _db.Checkins.Remove(checkin);
        await _db.SaveChangesAsync();
        return true;
    }
}
