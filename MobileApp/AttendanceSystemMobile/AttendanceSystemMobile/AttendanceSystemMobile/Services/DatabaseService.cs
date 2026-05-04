using AttendanceSystemMobile.Data;
using AttendanceSystemMobile.Models;
using Microsoft.EntityFrameworkCore;

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
            // Note: You should hash passwords in production!
            // This is simplified for learning purposes
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Enum == eNumber && u.PasswordHash == password);
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
    }
}