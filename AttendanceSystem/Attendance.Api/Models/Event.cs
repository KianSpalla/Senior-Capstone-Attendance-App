namespace Attendance.Api.Models;

public class Event
{
    public int eventId { get; set; }
    public string eventCode { get; set; } = string.Empty;
    public string eventName { get; set; } = string.Empty;
    public DateTime eventTime { get; set; }
    public string? eventLocation { get; set; }
    public int? capacity { get; set; }
    public string host { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime? createdAt { get; set; }

    public User? HostUser { get; set; }
    public ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
}
