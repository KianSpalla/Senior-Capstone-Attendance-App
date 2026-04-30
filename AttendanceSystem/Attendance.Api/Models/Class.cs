namespace Attendance.Api.Models;

public class Class
{
    public int classNo { get; set; }
    public string className { get; set; } = string.Empty;
    public string teacher { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }

    public User? TeacherUser { get; set; }
    public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
}
