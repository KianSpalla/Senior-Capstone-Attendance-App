using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemMobile.Models
{
    [Table("events")]
    public class Event
    {
        [Key]
        [Column("eventId")]
        public int EventId { get; set; }

        [Column("eventCode")]
        [StringLength(100)]
        public string EventCode { get; set; }

        [Column("eventName")]
        [StringLength(200)]
        public string EventName { get; set; }

        [Column("eventTime")]
        public DateTime EventTime { get; set; }

        [Column("eventLocation")]
        [StringLength(200)]
        public string EventLocation { get; set; }

        [Column("capacity")]
        public int? Capacity { get; set; }

        [Column("host")]
        [StringLength(7)]
        public string Host { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("Host")]
        public User HostUser { get; set; }

        public ICollection<CheckIn> CheckIns { get; set; }
    }
}