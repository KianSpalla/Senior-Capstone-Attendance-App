using AttendanceApp.Models;

namespace AttendanceApp.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int eventId);
        Task<Event?> GetByCodeAsync(string eventCode);
        Task<IEnumerable<Event>> GetByHostAsync(int hostUserId);
        Task<int> CreateAsync(Event evt);          // returns new eventId
        Task<bool> UpdateAsync(Event evt);
        Task<bool> DeleteAsync(int eventId);
    }

    public class EventRepository : IEventRepository
    {
        // create a constructor that takes in AppDbContext and assigns it to a private readonly field _db

        public Task<IEnumerable<Event>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Event?> GetByIdAsync(int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<Event?> GetByCodeAsync(string eventCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetByHostAsync(int hostUserId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(Event evt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Event evt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int eventId)
        {
            throw new NotImplementedException();
        }
    }
}