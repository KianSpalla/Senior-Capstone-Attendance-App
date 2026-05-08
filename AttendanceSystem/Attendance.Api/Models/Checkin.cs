namespace Attendance.Api.Models;

public class Checkin
{
    public int checkInId { get; set; }
    public int eventId { get; set; }
    public string Enum { get; set; } = string.Empty;
    public DateTime? checkedInAt { get; set; }

    public Event? Event { get; set; }
    public User? User { get; set; }
}
