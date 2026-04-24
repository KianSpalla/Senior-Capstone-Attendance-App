//Merge student and classes into this service or have both of them use the same one hereusing AttendanceApp.Models;
using AttendanceApp.Repositories;

namespace AttendanceApp.Services
{
    public interface IClassService
    {
        // Class CRUD
        Task<IEnumerable<Class>> GetAllClassesAsync();
        Task<Class?> GetClassByIdAsync(int classNo);
        Task<IEnumerable<Class>> GetClassesByTeacherAsync(int teacherUserId);
        Task<int> CreateClassAsync(Class cls);
        Task<bool> UpdateClassAsync(Class cls);
        Task<bool> DeleteClassAsync(int classNo);

        // StudentClass (enrollment) — merged per file map
        Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo);
        Task<IEnumerable<Class>> GetClassesForStudentAsync(int userId);
        Task<bool> EnrollStudentAsync(int userId, int classNo);
        Task<bool> UnenrollStudentAsync(int userId, int classNo);
    }

    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepo;

        public ClassService(IClassRepository classRepo)
        {
            _classRepo = classRepo;
        }

        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _classRepo.GetAllAsync();
        }

        public async Task<Class?> GetClassByIdAsync(int classNo)
        {
            return await _classRepo.GetByIdAsync(classNo);
        }

        public async Task<IEnumerable<Class>> GetClassesByTeacherAsync(int teacherUserId)
        {
            return await _classRepo.GetByTeacherAsync(teacherUserId);
        }

        public async Task<int> CreateClassAsync(Class cls)
        {
            return await _classRepo.CreateAsync(cls);
        }

        public async Task<bool> UpdateClassAsync(Class cls)
        {
            return await _classRepo.UpdateAsync(cls);
        }

        public async Task<bool> DeleteClassAsync(int classNo)
        {
            return await _classRepo.DeleteAsync(classNo);
        }

        public async Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo)
        {
            return await _classRepo.GetStudentsInClassAsync(classNo);
        }

        public async Task<IEnumerable<Class>> GetClassesForStudentAsync(int userId)
        {
            return await _classRepo.GetClassesForStudentAsync(userId);
        }

        public async Task<bool> EnrollStudentAsync(int userId, int classNo)
        {
            var alreadyEnrolled = await _classRepo.IsEnrolledAsync(userId, classNo);
            if (alreadyEnrolled) return false;
            return await _classRepo.EnrollStudentAsync(userId, classNo);
        }

        public async Task<bool> UnenrollStudentAsync(int userId, int classNo)
        {
            return await _classRepo.UnenrollStudentAsync(userId, classNo);
        }
    }
}