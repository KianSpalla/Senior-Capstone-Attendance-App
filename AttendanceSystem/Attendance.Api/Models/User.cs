using System;
namespace Attendance.Api.Models;
public enum UserRole
{
    admin,
    teacher,
    student
}

public class User
{
    public string Enum { get; set; } = string.Empty;
    public string fname { get; set; } = string.Empty;
    public string lname { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string? phoneNum { get; set; }
    public UserRole Role { get; set; }
    public DateTime? createdAt { get; set; }

    public ICollection<Class> ClassesTaught { get; set; } = new List<Class>();
    public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    public ICollection<Event> HostedEvents { get; set; } = new List<Event>();
    public ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
}
