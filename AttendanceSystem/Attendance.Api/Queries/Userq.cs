using AttendanceApp.Models;

namespace AttendanceApp.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEnumAsync(string enumValue);
        Task<int> CreateAsync(User user);           // returns new userId
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
    }

    public class UserRepository : IUserRepository
    {
        // create a constructor that takes in AppDbContext and assigns it to a private readonly field _db
        // private readonly AppDbContext _db; 
        // public UserRepository(AppDbContext db) { _db = db; } 

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEnumAsync(string enumValue)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}