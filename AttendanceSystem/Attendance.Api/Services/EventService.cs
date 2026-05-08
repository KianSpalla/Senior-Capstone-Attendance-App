using Attendance.Api.Models;
using Attendance.Api.Queries;

namespace Attendance.Api.Services;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int eventId);
    Task<Event?> GetEventByCodeAsync(string eventCode);
    Task<IEnumerable<Event>> GetEventsByHostAsync(string host);
    Task<EventSaveResult> CreateEventAsync(Event evt);
    Task<EventSaveResult> UpdateEventAsync(Event evt);
    Task<bool> DeleteEventAsync(int eventId);
    Task<EventAttendanceReport?> GetAttendanceReportAsync(int eventId);
    string GetQrCodeSvg(string eventCode);
    EventResponse ToResponse(Event evt);
}

public class EventService : IEventService
{
    private readonly IEventRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly IEventQrCodeService _qrCodeService;

    public EventService(IEventRepository repo, IUserRepository userRepo, IEventQrCodeService qrCodeService)
    {
        _repo = repo;
        _userRepo = userRepo;
        _qrCodeService = qrCodeService;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        return await _repo.GetByIdAsync(eventId);
    }

    public async Task<Event?> GetEventByCodeAsync(string eventCode)
    {
        return await _repo.GetByCodeAsync(eventCode.Trim().ToUpperInvariant());
    }

    public async Task<IEnumerable<Event>> GetEventsByHostAsync(string host)
    {
        return await _repo.GetByHostAsync(host.Trim().ToLowerInvariant());
    }

    public async Task<EventSaveResult> CreateEventAsync(Event evt)
    {
        var validation = await ValidateEventAsync(evt);

        if (validation is not EventSaveStatus.Success)
            return new EventSaveResult(validation);

        evt.eventCode = await GenerateEventCodeAsync();
        evt.createdAt ??= DateTime.UtcNow;

        var eventId = await _repo.CreateAsync(evt);
        return new EventSaveResult(EventSaveStatus.Success, eventId);
    }

    public async Task<EventSaveResult> UpdateEventAsync(Event evt)
    {
        var existing = await _repo.GetByIdAsync(evt.eventId);

        if (existing is null)
            return new EventSaveResult(EventSaveStatus.NotFound);

        var validation = await ValidateEventAsync(evt);

        if (validation is not EventSaveStatus.Success)
            return new EventSaveResult(validation);

        evt.eventCode = string.IsNullOrWhiteSpace(evt.eventCode)
            ? existing.eventCode
            : evt.eventCode.Trim().ToUpperInvariant();

        var updated = await _repo.UpdateAsync(evt);
        return new EventSaveResult(updated ? EventSaveStatus.Success : EventSaveStatus.NotFound, evt.eventId);
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        return await _repo.DeleteAsync(eventId);
    }

    public string GetQrCodeSvg(string eventCode)
    {
        return _qrCodeService.GenerateSvg(eventCode);
    }

    public EventResponse ToResponse(Event evt)
    {
        return new EventResponse(
            evt.eventId,
            evt.eventCode,
            evt.eventName,
            evt.eventTime,
            evt.eventLocation,
            evt.capacity,
            evt.host,
            evt.description,
            evt.createdAt,
            _qrCodeService.GetPayload(evt.eventCode),
            $"/api/Event/code/{Uri.EscapeDataString(evt.eventCode)}/qr"
        );
    }

    public async Task<EventAttendanceReport?> GetAttendanceReportAsync(int eventId)
    {
        var evt = await _repo.GetByIdWithCheckinsAsync(eventId);

        if (evt is null)
            return null;

        var attendees = evt.Checkins
            .OrderBy(c => c.checkedInAt)
            .Select(c => new EventAttendee(
                c.checkInId,
                c.Enum,
                c.User?.fname ?? string.Empty,
                c.User?.lname ?? string.Empty,
                c.User?.email ?? string.Empty,
                c.checkedInAt
            ))
            .ToList();

        return new EventAttendanceReport(
            evt.eventId,
            evt.eventCode,
            evt.eventName,
            evt.eventTime,
            evt.eventLocation,
            evt.capacity,
            attendees.Count,
            _qrCodeService.GetPayload(evt.eventCode),
            attendees
        );
    }

    private async Task<EventSaveStatus> ValidateEventAsync(Event evt)
    {
        if (string.IsNullOrWhiteSpace(evt.eventName))
            return EventSaveStatus.Invalid;

        if (evt.capacity is <= 0)
            return EventSaveStatus.Invalid;

        evt.host = evt.host.Trim().ToLowerInvariant();
        var host = await _userRepo.GetByEnumAsync(evt.host);

        if (host is null)
            return EventSaveStatus.HostNotFound;

        if (host.Role is not (UserRole.admin or UserRole.teacher))
            return EventSaveStatus.HostNotAllowed;

        return EventSaveStatus.Success;
    }

    private async Task<string> GenerateEventCodeAsync()
    {
        string code;

        do
        {
            code = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        }
        while (await _repo.GetByCodeAsync(code) is not null);

        return code;
    }
}

public enum EventSaveStatus
{
    Success,
    Invalid,
    NotFound,
    HostNotFound,
    HostNotAllowed
}

public record EventSaveResult(EventSaveStatus Status, int? EventId = null);

public record EventResponse(
    int eventId,
    string eventCode,
    string eventName,
    DateTime eventTime,
    string? eventLocation,
    int? capacity,
    string host,
    string? description,
    DateTime? createdAt,
    string qrCodePayload,
    string qrCodeSvgUrl
);

public record EventAttendanceReport(
    int eventId,
    string eventCode,
    string eventName,
    DateTime eventTime,
    string? eventLocation,
    int? capacity,
    int checkInCount,
    string qrCodePayload,
    IReadOnlyCollection<EventAttendee> attendees
);

public record EventAttendee(
    int checkInId,
    string Enum,
    string fname,
    string lname,
    string email,
    DateTime? checkedInAt
);
