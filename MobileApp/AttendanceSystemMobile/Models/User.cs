using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemMobile.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("enum")]
        [StringLength(7)]
        public string Enum { get; set; }

        [Column("fname")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Column("lname")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Column("email")]
        [StringLength(150)]
        public string Email { get; set; }

        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Column("phoneNum")]
        [StringLength(20)]
        public string PhoneNum { get; set; }

        [Column("role")]
        [StringLength(10)]
        public string Role { get; set; } // admin, teacher, student

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Class> TaughtClasses { get; set; }
        public ICollection<Event> HostedEvents { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; }
        public ICollection<CheckIn> CheckIns { get; set; }
    }
}