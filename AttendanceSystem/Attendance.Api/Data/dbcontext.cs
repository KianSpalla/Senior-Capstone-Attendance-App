using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Data;

public class AttendanceDbContext : DbContext
{
    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
        : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<StudentClass> StudentClasses => Set<StudentClass>();
    public DbSet<Checkin> Checkins => Set<Checkin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Enum);

            entity.Property(u => u.Enum).HasColumnName("enum").HasMaxLength(7);
            entity.Property(u => u.fname).HasColumnName("fname").HasMaxLength(100);
            entity.Property(u => u.lname).HasColumnName("lname").HasMaxLength(100);
            entity.Property(u => u.email).HasColumnName("email").HasMaxLength(150);
            entity.Property(u => u.password).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(u => u.phoneNum).HasColumnName("phoneNum").HasMaxLength(20);
            entity.Property(u => u.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(50);
            entity.Property(u => u.createdAt).HasColumnName("createdAt");

            entity.HasIndex(u => u.email).IsUnique();
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("class");
            entity.HasKey(c => c.classNo);

            entity.Property(c => c.classNo).HasColumnName("classNo");
            entity.Property(c => c.className).HasColumnName("className").HasMaxLength(150);
            entity.Property(c => c.teacher).HasColumnName("teacher").HasMaxLength(7);
            entity.Property(c => c.createdAt).HasColumnName("createdAt");

            entity.HasOne(c => c.TeacherUser)
                .WithMany(u => u.ClassesTaught)
                .HasForeignKey(c => c.teacher)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.eventId);

            entity.Property(e => e.eventId).HasColumnName("eventId");
            entity.Property(e => e.eventCode).HasColumnName("eventCode").HasMaxLength(100);
            entity.Property(e => e.eventName).HasColumnName("eventName").HasMaxLength(200);
            entity.Property(e => e.eventTime).HasColumnName("eventTime");
            entity.Property(e => e.eventLocation).HasColumnName("eventLocation").HasMaxLength(200);
            entity.Property(e => e.capacity).HasColumnName("capacity");
            entity.Property(e => e.host).HasColumnName("host").HasMaxLength(7);
            entity.Property(e => e.description).HasColumnName("description");
            entity.Property(e => e.createdAt).HasColumnName("createdAt");

            entity.HasIndex(e => e.eventCode).IsUnique();

            entity.HasOne(e => e.HostUser)
                .WithMany(u => u.HostedEvents)
                .HasForeignKey(e => e.host)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
            entity.ToTable("studentclass");
            entity.HasKey(sc => new { sc.Enum, sc.classNo });

            entity.Property(sc => sc.Enum).HasColumnName("enum").HasMaxLength(7);
            entity.Property(sc => sc.classNo).HasColumnName("classNo");

            entity.HasOne(sc => sc.User)
                .WithMany(u => u.StudentClasses)
                .HasForeignKey(sc => sc.Enum)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sc => sc.Class)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(sc => sc.classNo)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Checkin>(entity =>
        {
            entity.ToTable("checkins");
            entity.HasKey(c => c.checkInId);

            entity.Property(c => c.checkInId).HasColumnName("checkInId");
            entity.Property(c => c.eventId).HasColumnName("eventId");
            entity.Property(c => c.Enum).HasColumnName("enum").HasMaxLength(7);
            entity.Property(c => c.checkedInAt).HasColumnName("checkedInAt");

            entity.HasIndex(c => new { c.eventId, c.Enum }).IsUnique();

            entity.HasOne(c => c.Event)
                .WithMany(e => e.Checkins)
                .HasForeignKey(c => c.eventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Checkins)
                .HasForeignKey(c => c.Enum)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
