using AttendanceApp.Models;

namespace AttendanceApp.Repositories
{
    public interface ICheckInRepository
    {
        Task<IEnumerable<CheckIn>> GetByEventAsync(int eventId);
        Task<IEnumerable<CheckIn>> GetByUserAsync(int userId);
        Task<CheckIn?> GetAsync(int eventId, int userId);
        Task<int> GetCheckInCountAsync(int eventId);
        Task<int> CreateAsync(CheckIn checkIn);    // returns new checkInId
        Task<bool> DeleteAsync(int checkInId);
        Task<bool> DeleteByEventAndUserAsync(int eventId, int userId);
    }

    public class CheckInRepository : ICheckInRepository
    {
        // create a constructor that takes in AppDbContext and assigns it to a private readonly field _db

        public Task<IEnumerable<CheckIn>> GetByEventAsync(int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CheckIn>> GetByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<CheckIn?> GetAsync(int eventId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCheckInCountAsync(int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(CheckIn checkIn)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int checkInId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByEventAndUserAsync(int eventId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}