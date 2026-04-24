using System;
namespace Attendance.Api.Models
{
    public class Checkin
    {
        public int checkinId { get; set; }
        public int eventId { get; set; } //foreign key to Event event.eventId
        public int userId { get; set; } // foreign key to User user.userId
        public DateTime checkinTime { get; set; }

        //navigation properties to Event and User
        public Event? Event { get; set; }
        public User? User { get; set; }
    }
}