using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemMobile.Models
{
    [Table("checkins")]
    public class CheckIn
    {
        [Key]
        [Column("checkInId")]
        public int CheckInId { get; set; }

        [Column("eventId")]
        public int EventId { get; set; }

        [Column("enum")]
        [StringLength(7)]
        public string Enum { get; set; }

        [Column("checkedInAt")]
        public DateTime CheckedInAt { get; set; }

        // Navigation properties
        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("Enum")]
        public User User { get; set; }
    }
}