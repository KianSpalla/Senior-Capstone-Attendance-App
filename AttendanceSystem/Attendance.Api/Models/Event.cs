using System;
namespace Attendance.Api.Models
{
    public class Events {
        public int eventId { get; set; }
        public string eventCode { get; set; }
        public string eventName { get; set; }
        public DateTime EventTime { get; set; }    
        public Location EventLocation { get; set; }
        public int? Capacity { get; set; }
        public int Host { get; set; } //foreign key to users table
        public string? Description { get; set; }    
        public DateTime createdAt { get; set; }

        // Navigation properties to get the host user details
        public User? HostUser { get; set; }
    }
}