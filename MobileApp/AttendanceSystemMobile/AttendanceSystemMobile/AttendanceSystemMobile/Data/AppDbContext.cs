using Microsoft.EntityFrameworkCore;
using AttendanceSystemMobile.Models;

namespace AttendanceSystemMobile.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets represent tables
        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for StudentClass
            modelBuilder.Entity<StudentClass>()
                .HasKey(sc => new { sc.Enum, sc.ClassNo });

            // Configure relationships
            modelBuilder.Entity<Class>()
                .HasOne(c => c.TeacherUser)
                .WithMany(u => u.TaughtClasses)
                .HasForeignKey(c => c.Teacher)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.HostUser)
                .WithMany(u => u.HostedEvents)
                .HasForeignKey(e => e.Host)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Student)
                .WithMany(u => u.StudentClasses)
                .HasForeignKey(sc => sc.Enum)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(sc => sc.ClassNo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CheckIn>()
                .HasOne(ci => ci.Event)
                .WithMany(e => e.CheckIns)
                .HasForeignKey(ci => ci.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CheckIn>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CheckIns)
                .HasForeignKey(ci => ci.Enum)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}