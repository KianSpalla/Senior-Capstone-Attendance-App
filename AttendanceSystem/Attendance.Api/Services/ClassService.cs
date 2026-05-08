using Attendance.Api.Models;
using Attendance.Api.Queries;

namespace Attendance.Api.Services;

public interface IClassService
{
    Task<IEnumerable<Class>> GetAllClassesAsync();
    Task<Class?> GetClassByIdAsync(int classNo);
    Task<IEnumerable<Class>> GetClassesByTeacherAsync(string teacher);
    Task<ClassSaveResult> CreateClassAsync(Class cls);
    Task<ClassSaveResult> UpdateClassAsync(Class cls);
    Task<bool> DeleteClassAsync(int classNo);

    Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo);
    Task<IEnumerable<Class>> GetClassesForStudentAsync(string enumValue);
    Task<EnrollmentResult> EnrollStudentAsync(string enumValue, int classNo);
    Task<bool> UnenrollStudentAsync(string enumValue, int classNo);
}

public class ClassService : IClassService
{
    private readonly IClassRepository _repo;
    private readonly IUserRepository _userRepo;

    public ClassService(IClassRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<Class>> GetAllClassesAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Class?> GetClassByIdAsync(int classNo)
    {
        return await _repo.GetByIdAsync(classNo);
    }

    public async Task<IEnumerable<Class>> GetClassesByTeacherAsync(string teacher)
    {
        return await _repo.GetByTeacherAsync(teacher);
    }

    public async Task<ClassSaveResult> CreateClassAsync(Class cls)
    {
        var validation = await ValidateClassAsync(cls);

        if (validation is not ClassSaveStatus.Success)
            return new ClassSaveResult(validation);

        cls.createdAt ??= DateTime.UtcNow;
        var classNo = await _repo.CreateAsync(cls);
        return new ClassSaveResult(ClassSaveStatus.Success, classNo);
    }

    public async Task<ClassSaveResult> UpdateClassAsync(Class cls)
    {
        var existing = await _repo.GetByIdAsync(cls.classNo);

        if (existing is null)
            return new ClassSaveResult(ClassSaveStatus.NotFound);

        var validation = await ValidateClassAsync(cls);

        if (validation is not ClassSaveStatus.Success)
            return new ClassSaveResult(validation);

        var updated = await _repo.UpdateAsync(cls);
        return new ClassSaveResult(updated ? ClassSaveStatus.Success : ClassSaveStatus.NotFound, cls.classNo);
    }

    public async Task<bool> DeleteClassAsync(int classNo)
    {
        return await _repo.DeleteAsync(classNo);
    }

    public async Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo)
    {
        return await _repo.GetStudentsInClassAsync(classNo);
    }

    public async Task<IEnumerable<Class>> GetClassesForStudentAsync(string enumValue)
    {
        return await _repo.GetClassesForStudentAsync(enumValue);
    }

    public async Task<EnrollmentResult> EnrollStudentAsync(string enumValue, int classNo)
    {
        var cls = await _repo.GetByIdAsync(classNo);

        if (cls is null)
            return new EnrollmentResult(EnrollmentStatus.ClassNotFound);

        var student = await _userRepo.GetByEnumAsync(enumValue);

        if (student is null)
            return new EnrollmentResult(EnrollmentStatus.UserNotFound);

        if (student.Role is not UserRole.student)
            return new EnrollmentResult(EnrollmentStatus.UserNotStudent);

        var enrolled = await _repo.EnrollStudentAsync(enumValue, classNo);
        return new EnrollmentResult(enrolled ? EnrollmentStatus.Success : EnrollmentStatus.AlreadyEnrolled);
    }

    public async Task<bool> UnenrollStudentAsync(string enumValue, int classNo)
    {
        return await _repo.UnenrollStudentAsync(enumValue, classNo);
    }

    private async Task<ClassSaveStatus> ValidateClassAsync(Class cls)
    {
        if (string.IsNullOrWhiteSpace(cls.className))
            return ClassSaveStatus.Invalid;

        cls.teacher = cls.teacher.Trim().ToLowerInvariant();
        var teacher = await _userRepo.GetByEnumAsync(cls.teacher);

        if (teacher is null)
            return ClassSaveStatus.TeacherNotFound;

        if (teacher.Role is not (UserRole.admin or UserRole.teacher))
            return ClassSaveStatus.TeacherNotAllowed;

        return ClassSaveStatus.Success;
    }
}

public enum ClassSaveStatus
{
    Success,
    Invalid,
    NotFound,
    TeacherNotFound,
    TeacherNotAllowed
}

public record ClassSaveResult(ClassSaveStatus Status, int? ClassNo = null);

public enum EnrollmentStatus
{
    Success,
    ClassNotFound,
    UserNotFound,
    UserNotStudent,
    AlreadyEnrolled
}

public record EnrollmentResult(EnrollmentStatus Status);
