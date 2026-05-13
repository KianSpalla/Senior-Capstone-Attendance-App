using Attendance.Api.Data;
using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Services;

public interface IAttendanceReportService
{
    Task<StudentAttendanceProgressReport> GetStudentProgressAsync(int? classNo, int? eventId, string? status);
}

public class AttendanceReportService : IAttendanceReportService
{
    private const int RequiredEventsPerSemester = 3;
    private readonly AttendanceDbContext _db;

    public AttendanceReportService(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<StudentAttendanceProgressReport> GetStudentProgressAsync(int? classNo, int? eventId, string? status)
    {
        var normalizedStatus = status?.Trim().ToLowerInvariant();
        var studentsQuery = _db.Users
            .AsNoTracking()
            .Where(user => user.Role == UserRole.student);

        if (classNo is not null)
        {
            studentsQuery = studentsQuery.Where(user =>
                _db.StudentClasses.Any(sc => sc.Enum == user.Enum && sc.classNo == classNo));
        }

        var students = await studentsQuery
            .OrderBy(user => user.lname)
            .ThenBy(user => user.fname)
            .ToListAsync();
        var studentEnums = students.Select(student => student.Enum).ToList();

        var checkins = await _db.Checkins
            .AsNoTracking()
            .Include(checkin => checkin.Event)
            .Where(checkin => studentEnums.Contains(checkin.Enum))
            .ToListAsync();

        var classMemberships = await _db.StudentClasses
            .AsNoTracking()
            .Include(sc => sc.Class)
            .Where(sc => studentEnums.Contains(sc.Enum))
            .ToListAsync();

        var rows = students.Select(student =>
        {
            var studentCheckins = checkins
                .Where(checkin => checkin.Enum == student.Enum)
                .ToList();
            var attendedCount = studentCheckins
                .Select(checkin => checkin.eventId)
                .Distinct()
                .Count();
            var selectedEventCheckin = eventId is null
                ? null
                : studentCheckins.FirstOrDefault(checkin => checkin.eventId == eventId);
            var attendedSelectedEvent = selectedEventCheckin is not null;
            var checkinHistory = studentCheckins
                .GroupBy(checkin => checkin.eventId)
                .Select(group => group
                    .OrderBy(checkin => checkin.checkedInAt)
                    .First())
                .OrderBy(checkin => checkin.checkedInAt)
                .Select(checkin => new StudentCheckinHistoryItem(
                    checkin.eventId,
                    checkin.Event?.eventCode ?? string.Empty,
                    checkin.Event?.eventName ?? string.Empty,
                    checkin.checkedInAt
                ))
                .ToList();
            var classNames = classMemberships
                .Where(sc => sc.Enum == student.Enum && sc.Class is not null)
                .Select(sc => sc.Class!.className)
                .OrderBy(className => className)
                .ToList();

            return new StudentAttendanceProgress(
                student.Enum,
                student.fname,
                student.lname,
                student.email,
                student.phoneNum,
                classNames,
                studentCheckins
                    .Select(checkin => checkin.eventId)
                    .Distinct()
                    .OrderBy(id => id)
                    .ToList(),
                attendedCount,
                RequiredEventsPerSemester,
                Math.Max(RequiredEventsPerSemester - attendedCount, 0),
                attendedCount >= RequiredEventsPerSemester,
                eventId,
                attendedSelectedEvent,
                selectedEventCheckin?.checkedInAt,
                checkinHistory
            );
        });

        rows = normalizedStatus switch
        {
            "complete" => rows.Where(row => row.isComplete),
            "needs" => rows.Where(row => !row.isComplete),
            "attended-event" => rows.Where(row => row.attendedSelectedEvent),
            "missed-event" => eventId is null ? rows : rows.Where(row => !row.attendedSelectedEvent),
            _ => rows
        };

        var rowList = rows.ToList();

        return new StudentAttendanceProgressReport(
            RequiredEventsPerSemester,
            classNo,
            eventId,
            normalizedStatus ?? "all",
            rowList.Count,
            rowList.Count(row => row.isComplete),
            rowList.Count(row => !row.isComplete),
            rowList
        );
    }
}

public record StudentAttendanceProgressReport(
    int requiredEventsPerSemester,
    int? classNo,
    int? eventId,
    string status,
    int studentCount,
    int completeCount,
    int needsCount,
    IReadOnlyCollection<StudentAttendanceProgress> students
);

public record StudentAttendanceProgress(
    string studentEnum,
    string fname,
    string lname,
    string email,
    string? phoneNum,
    IReadOnlyCollection<string> classes,
    IReadOnlyCollection<int> attendedEventIds,
    int attendedCount,
    int requiredCount,
    int remainingCount,
    bool isComplete,
    int? selectedEventId,
    bool attendedSelectedEvent,
    DateTime? selectedEventCheckedInAt,
    IReadOnlyCollection<StudentCheckinHistoryItem> checkins
);

public record StudentCheckinHistoryItem(
    int eventId,
    string eventCode,
    string eventName,
    DateTime? checkedInAt
);
