using AttendanceApp.Models;
using AttendanceApp.Repositories;

namespace AttendanceApp.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<Event?> GetEventByCodeAsync(string eventCode);
        Task<IEnumerable<Event>> GetEventsByHostAsync(int hostUserId);
        Task<int> CreateEventAsync(Event evt);
        Task<bool> UpdateEventAsync(Event evt);
        Task<bool> DeleteEventAsync(int eventId);
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;

        public EventService(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepo.GetAllAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _eventRepo.GetByIdAsync(eventId);
        }

        public async Task<Event?> GetEventByCodeAsync(string eventCode)
        {
            return await _eventRepo.GetByCodeAsync(eventCode);
        }

        public async Task<IEnumerable<Event>> GetEventsByHostAsync(int hostUserId)
        {
            return await _eventRepo.GetByHostAsync(hostUserId);
        }

        public async Task<int> CreateEventAsync(Event evt)
        {
            // TODO: validate eventCode uniqueness, host exists, etc.
            return await _eventRepo.CreateAsync(evt);
        }

        public async Task<bool> UpdateEventAsync(Event evt)
        {
            return await _eventRepo.UpdateAsync(evt);
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            return await _eventRepo.DeleteAsync(eventId);
        }
    }
}