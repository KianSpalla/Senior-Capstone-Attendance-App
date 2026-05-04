using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemMobile.Models
{
    [Table("studentclass")]
    public class StudentClass
    {
        [Column("enum")]
        [StringLength(7)]
        public string Enum { get; set; }

        [Column("classNo")]
        public int ClassNo { get; set; }

        // Navigation properties
        [ForeignKey("Enum")]
        public User Student { get; set; }

        [ForeignKey("ClassNo")]
        public Class Class { get; set; }
    }
}