using Attendance.Api.Models;
using Attendance.Api.Queries;

namespace Attendance.Api.Services;

public interface ICheckInService
{
    Task<IEnumerable<Checkin>> GetCheckInsForEventAsync(int eventId);
    Task<IEnumerable<Checkin>> GetCheckInsForUserAsync(string enumValue);
    Task<int> GetCheckInCountAsync(int eventId);
    Task<CheckInResult> CheckInAsync(int eventId, string enumValue);
    Task<CheckInResult> CheckInByEventCodeAsync(string eventCode, string enumValue);
    Task<bool> RemoveCheckInAsync(int checkInId);
}

public class CheckInService : ICheckInService
{
    private readonly ICheckinRepository _repo;
    private readonly IEventRepository _eventRepo;
    private readonly IUserRepository _userRepo;

    public CheckInService(ICheckinRepository repo, IEventRepository eventRepo, IUserRepository userRepo)
    {
        _repo = repo;
        _eventRepo = eventRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<Checkin>> GetCheckInsForEventAsync(int eventId)
    {
        return await _repo.GetByEventAsync(eventId);
    }

    public async Task<IEnumerable<Checkin>> GetCheckInsForUserAsync(string enumValue)
    {
        return await _repo.GetByUserAsync(enumValue);
    }

    public async Task<int> GetCheckInCountAsync(int eventId)
    {
        return await _repo.GetCheckinCountAsync(eventId);
    }

    public async Task<CheckInResult> CheckInAsync(int eventId, string enumValue)
    {
        var evt = await _eventRepo.GetByIdAsync(eventId);

        if (evt is null)
            return new CheckInResult(CheckInStatus.EventNotFound);

        return await CheckInForEventAsync(evt, enumValue);
    }

    public async Task<CheckInResult> CheckInByEventCodeAsync(string eventCode, string enumValue)
    {
        var evt = await _eventRepo.GetByCodeAsync(eventCode.Trim().ToUpperInvariant());

        if (evt is null)
            return new CheckInResult(CheckInStatus.EventNotFound);

        return await CheckInForEventAsync(evt, enumValue);
    }

    public async Task<bool> RemoveCheckInAsync(int checkInId)
    {
        return await _repo.DeleteAsync(checkInId);
    }

    private async Task<CheckInResult> CheckInForEventAsync(Event evt, string enumValue)
    {
        enumValue = enumValue.Trim().ToLowerInvariant();
        var user = await _userRepo.GetByEnumAsync(enumValue);

        if (user is null)
            return new CheckInResult(CheckInStatus.UserNotFound);

        if (user.Role is not UserRole.student)
            return new CheckInResult(CheckInStatus.UserNotAllowed);

        var existing = await _repo.GetAsync(evt.eventId, enumValue);

        if (existing is not null)
            return new CheckInResult(CheckInStatus.AlreadyCheckedIn, existing.checkInId);

        if (evt.capacity is not null)
        {
            var count = await _repo.GetCheckinCountAsync(evt.eventId);

            if (count >= evt.capacity)
                return new CheckInResult(CheckInStatus.EventAtCapacity);
        }

        var checkin = new Checkin
        {
            eventId = evt.eventId,
            Enum = enumValue,
            checkedInAt = DateTime.UtcNow
        };

        var checkInId = await _repo.CreateAsync(checkin);
        return new CheckInResult(CheckInStatus.Success, checkInId);
    }
}

public enum CheckInStatus
{
    Success,
    EventNotFound,
    UserNotFound,
    UserNotAllowed,
    AlreadyCheckedIn,
    EventAtCapacity
}

public record CheckInResult(CheckInStatus Status, int? CheckInId = null);
