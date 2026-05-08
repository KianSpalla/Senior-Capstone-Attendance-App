using AttendanceSystemMobile.Data;
using AttendanceSystemMobile.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AttendanceSystemMobile.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;

        public DatabaseService(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- LOGIN ----------------

        public async Task<User> LoginAsync(string eNumber, string password)
        {
            try
            {
                Debug.WriteLine($"[DATABASE] Login attempt for: {eNumber}");
                
                await _context.Database.CanConnectAsync();
                Debug.WriteLine("[DATABASE] Database connection successful");
                
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Enum == eNumber);

                if (user == null)
                {
                    Debug.WriteLine("[DATABASE] User not found");
                    return null;
                }

                // Check if password needs initialization
                if (user.PasswordHash == "NO_WEB_LOGIN")
                {
                    Debug.WriteLine("[DATABASE] Password not initialized");
                    return null; // Will be handled by LoginPage
                }

                // Verify hashed password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (passwordValid)
                {
                    Debug.WriteLine("[DATABASE] Login successful");
                    return user;
                }

                Debug.WriteLine("[DATABASE] Invalid password");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DATABASE] Login error: {ex.Message}");
                throw;
            }
        }

        // ---------------- EVENTS ----------------

        public async Task<List<Event>> GetEventsAsync()
        {
            return await _context.Events
                .OrderBy(e => e.EventTime)
                .ToListAsync();
        }

        public async Task<Event> GetEventByCodeAsync(string eventCode)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.EventCode == eventCode);
        }

        // ---------------- ATTENDANCE / CHECK-INS ----------------

        public async Task<int> GetAttendanceCountAsync(string studentEnum)
        {
            return await _context.CheckIns
                .Where(c => c.Enum == studentEnum)
                .Select(c => c.EventId)
                .Distinct()
                .CountAsync();
        }

        public async Task<bool> RegisterAttendanceAsync(string studentEnum, int eventId)
        {
            bool exists = await _context.CheckIns
                .AnyAsync(c => c.Enum == studentEnum && c.EventId == eventId);

            if (exists)
                return false;

            _context.CheckIns.Add(new CheckIn
            {
                Enum = studentEnum,
                EventId = eventId,
                CheckedInAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------- USER MANAGEMENT ----------------

        public async Task<User> GetUserAsync(string enumNumber)
        {
            return await _context.Users.FindAsync(enumNumber);
        }

        public async Task<List<User>> GetStudentsInClassAsync(int classNo)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassNo == classNo)
                .Include(sc => sc.Student)
                .Select(sc => sc.Student)
                .ToListAsync();
        }

        // ---------------- CLASS MANAGEMENT ----------------

        public async Task<List<Class>> GetClassesByTeacherAsync(string teacherEnum)
        {
            return await _context.Classes
                .Where(c => c.Teacher == teacherEnum)
                .ToListAsync();
        }

        public async Task<List<Class>> GetClassesByStudentAsync(string studentEnum)
        {
            return await _context.StudentClasses
                .Where(sc => sc.Enum == studentEnum)
                .Include(sc => sc.Class)
                .Select(sc => sc.Class)
                .ToListAsync();
        }
        // Add this method to your existing DatabaseService class

        /// <summary>
        /// Updates a user's password (used for initialization)
        /// </summary>
        public async Task<bool> UpdateUserPasswordAsync(string userEnum, string newPasswordHash)
        {
            try
            {
                Debug.WriteLine($"[DATABASE] Updating password for user: {userEnum}");

                var user = await _context.Users.FindAsync(userEnum);

                if (user == null)
                {
                    Debug.WriteLine($"[DATABASE] User not found: {userEnum}");
                    return false;
                }

                user.PasswordHash = newPasswordHash;

                await _context.SaveChangesAsync();

                Debug.WriteLine("[DATABASE] Password updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DATABASE] UpdateUserPasswordAsync error: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Gets a user by their enum number (for login checks)
        /// </summary>
        public async Task<User> GetUserByEnumAsync(string userEnum)
        {
            try
            {
                Debug.WriteLine($"[DATABASE] Getting user: {userEnum}");
                return await _context.Users.FindAsync(userEnum);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DATABASE] GetUserByEnumAsync error: {ex.Message}");
                throw;
            }
        }
    }
}