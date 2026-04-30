namespace Attendance.Api.Models
{
    public class StudentClass
    {
        public string Enum { get; set; } = string.Empty;
        public int classNo { get; set; }

        public User? User { get; set; }
        public Class? Class { get; set; }
    }
}
