using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemMobile.Models
{
    [Table("class")]
    public class Class
    {
        [Key]
        [Column("classNo")]
        public int ClassNo { get; set; }

        [Column("className")]
        [StringLength(150)]
        public string ClassName { get; set; }

        [Column("teacher")]
        [StringLength(7)]
        public string Teacher { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("Teacher")]
        public User TeacherUser { get; set; }

        public ICollection<StudentClass> StudentClasses { get; set; }
    }
}