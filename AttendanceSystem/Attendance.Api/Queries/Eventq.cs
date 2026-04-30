using Attendance.Api.Data;
using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Queries;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int eventId);
    Task<Event?> GetByIdWithCheckinsAsync(int eventId);
    Task<Event?> GetByCodeAsync(string eventCode);
    Task<IEnumerable<Event>> GetByHostAsync(string host);
    Task<int> CreateAsync(Event evt);
    Task<bool> UpdateAsync(Event evt);
    Task<bool> DeleteAsync(int eventId);
}

public class EventRepository : IEventRepository
{
    private readonly AttendanceDbContext _db;

    public EventRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _db.Events
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int eventId)
    {
        return await _db.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.eventId == eventId);
    }

    public async Task<Event?> GetByIdWithCheckinsAsync(int eventId)
    {
        return await _db.Events
            .AsNoTracking()
            .Include(e => e.Checkins)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(e => e.eventId == eventId);
    }

    public async Task<Event?> GetByCodeAsync(string eventCode)
    {
        return await _db.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.eventCode == eventCode);
    }

    public async Task<IEnumerable<Event>> GetByHostAsync(string host)
    {
        return await _db.Events
            .AsNoTracking()
            .Where(e => e.host == host)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(Event evt)
    {
        _db.Events.Add(evt);
        await _db.SaveChangesAsync();
        return evt.eventId;
    }

    public async Task<bool> UpdateAsync(Event evt)
    {
        var existingEvent = await _db.Events.FindAsync(evt.eventId);

        if (existingEvent is null)
            return false;

        existingEvent.eventCode = evt.eventCode;
        existingEvent.eventName = evt.eventName;
        existingEvent.eventTime = evt.eventTime;
        existingEvent.eventLocation = evt.eventLocation;
        existingEvent.capacity = evt.capacity;
        existingEvent.host = evt.host;
        existingEvent.description = evt.description;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int eventId)
    {
        var evt = await _db.Events.FindAsync(eventId);

        if (evt is null)
            return false;

        _db.Events.Remove(evt);
        await _db.SaveChangesAsync();
        return true;
    }
}
