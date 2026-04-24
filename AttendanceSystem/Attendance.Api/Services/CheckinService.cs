using AttendanceApp.Models;
using AttendanceApp.Repositories;

namespace AttendanceApp.Services
{
    public interface ICheckInService
    {
        Task<IEnumerable<CheckIn>> GetCheckInsForEventAsync(int eventId);
        Task<IEnumerable<CheckIn>> GetCheckInsForUserAsync(int userId);
        Task<int> GetCheckInCountAsync(int eventId);
        /// <summary>Check a user into an event. Returns the new checkInId, or -1 if already checked in / at capacity.</summary>
        Task<int> CheckInAsync(int eventId, int userId);
        Task<bool> RemoveCheckInAsync(int checkInId);
    }

    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepo;
        private readonly IEventRepository _eventRepo;

        public CheckInService(ICheckInRepository checkInRepo, IEventRepository eventRepo)
        {
            _checkInRepo = checkInRepo;
            _eventRepo = eventRepo;
        }

        public async Task<IEnumerable<CheckIn>> GetCheckInsForEventAsync(int eventId)
        {
            return await _checkInRepo.GetByEventAsync(eventId);
        }

        public async Task<IEnumerable<CheckIn>> GetCheckInsForUserAsync(int userId)
        {
            return await _checkInRepo.GetByUserAsync(userId);
        }

        public async Task<int> GetCheckInCountAsync(int eventId)
        {
            return await _checkInRepo.GetCheckInCountAsync(eventId);
        }

        public async Task<int> CheckInAsync(int eventId, int userId)
        {
            // Duplicate check — DB also enforces unique(eventId, userId)
            var existing = await _checkInRepo.GetAsync(eventId, userId);
            if (existing != null) return -1;

            // Capacity check
            var evt = await _eventRepo.GetByIdAsync(eventId);
            if (evt?.Capacity != null)
            {
                var count = await _checkInRepo.GetCheckInCountAsync(eventId);
                if (count >= evt.Capacity) return -1;
            }

            var checkIn = new CheckIn { EventId = eventId, UserId = userId };
            return await _checkInRepo.CreateAsync(checkIn);
        }

        public async Task<bool> RemoveCheckInAsync(int checkInId)
        {
            return await _checkInRepo.DeleteAsync(checkInId);
        }
    }
}