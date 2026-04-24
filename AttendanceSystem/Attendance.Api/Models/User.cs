using System;
using Attendance.Api.Models;

namespace Attendance.Api.Models
{
    public enum UserRole
    {
        Admin, Teacher, Student
    }
}
public class User {
    public int userId { get; set; }
    public string? Enum { get; set; }
    public string fname { get; set; }
    public string lname { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string? phoneNum { get; set; }
    public UserRole Role { get; set; }
    public DateTime? createdAt {get; set; }
    
}