using AttendanceApp.Models;
using AttendanceApp.Repositories;

namespace AttendanceApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> CreateUserAsync(User user, string plainTextPassword);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> ValidateCredentialsAsync(string email, string plainTextPassword);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepo.GetByIdAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepo.GetByEmailAsync(email);
        }

        public async Task<int> CreateUserAsync(User user, string plainTextPassword)
        {
            // TODO: hash plainTextPassword and assign to user.PasswordHash
            // e.g. user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
            return await _userRepo.CreateAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepo.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepo.DeleteAsync(userId);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string plainTextPassword)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return false;
            // TODO: verify hash e.g. BCrypt.Net.BCrypt.Verify(plainTextPassword, user.PasswordHash)
            // return true if valid, false if not
            throw new NotImplementedException();
        }
    }
}