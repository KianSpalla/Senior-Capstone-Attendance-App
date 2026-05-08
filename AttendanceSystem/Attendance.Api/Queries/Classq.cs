using Attendance.Api.Data;
using Attendance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Api.Queries;

public interface IClassRepository
{
    Task<IEnumerable<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(int classNo);
    Task<IEnumerable<Class>> GetByTeacherAsync(string teacher);
    Task<int> CreateAsync(Class cls);
    Task<bool> UpdateAsync(Class cls);
    Task<bool> DeleteAsync(int classNo);

    Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo);
    Task<IEnumerable<Class>> GetClassesForStudentAsync(string enumValue);
    Task<bool> EnrollStudentAsync(string enumValue, int classNo);
    Task<bool> UnenrollStudentAsync(string enumValue, int classNo);
    Task<bool> IsEnrolledAsync(string enumValue, int classNo);
}

public class ClassRepository : IClassRepository
{
    private readonly AttendanceDbContext _db;

    public ClassRepository(AttendanceDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _db.Classes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Class?> GetByIdAsync(int classNo)
    {
        return await _db.Classes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.classNo == classNo);
    }

    public async Task<IEnumerable<Class>> GetByTeacherAsync(string teacher)
    {
        return await _db.Classes
            .AsNoTracking()
            .Where(c => c.teacher == teacher)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(Class cls)
    {
        _db.Classes.Add(cls);
        await _db.SaveChangesAsync();
        return cls.classNo;
    }

    public async Task<bool> UpdateAsync(Class cls)
    {
        var existingClass = await _db.Classes.FindAsync(cls.classNo);

        if (existingClass is null)
            return false;

        existingClass.className = cls.className;
        existingClass.teacher = cls.teacher;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int classNo)
    {
        var cls = await _db.Classes.FindAsync(classNo);

        if (cls is null)
            return false;

        _db.Classes.Remove(cls);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo)
    {
        return await _db.StudentClasses
            .AsNoTracking()
            .Where(sc => sc.classNo == classNo)
            .Select(sc => sc.User!)
            .ToListAsync();
    }

    public async Task<IEnumerable<Class>> GetClassesForStudentAsync(string enumValue)
    {
        return await _db.StudentClasses
            .AsNoTracking()
            .Where(sc => sc.Enum == enumValue)
            .Select(sc => sc.Class!)
            .ToListAsync();
    }

    public async Task<bool> EnrollStudentAsync(string enumValue, int classNo)
    {
        var alreadyEnrolled = await IsEnrolledAsync(enumValue, classNo);

        if (alreadyEnrolled)
            return false;

        _db.StudentClasses.Add(new StudentClass
        {
            Enum = enumValue,
            classNo = classNo
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnenrollStudentAsync(string enumValue, int classNo)
    {
        var enrollment = await _db.StudentClasses.FindAsync(enumValue, classNo);

        if (enrollment is null)
            return false;

        _db.StudentClasses.Remove(enrollment);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsEnrolledAsync(string enumValue, int classNo)
    {
        return await _db.StudentClasses
            .AnyAsync(sc => sc.Enum == enumValue && sc.classNo == classNo);
    }
}
