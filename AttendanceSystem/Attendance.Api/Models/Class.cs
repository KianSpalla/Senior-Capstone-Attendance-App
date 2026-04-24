using System;
namespace Attendance.Api.Models
{
    public class Class
    {
        public int classNum { get; set; }
        public string className { get; set; } = string.Empty;
        public int Teacher { get; set; } //foreign key to users table
        public DateTime createdAt { get; set; }

        // Navigation property to get the list of events associated with this class
        public User? TeacherUser { get; set; }
        public ICollection<StudentClass>? StudentClasses { get; set; }
    }
}