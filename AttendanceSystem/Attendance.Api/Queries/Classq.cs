using AttendanceApp.Models;

namespace AttendanceApp.Repositories
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<Class?> GetByIdAsync(int classNo);
        Task<IEnumerable<Class>> GetByTeacherAsync(int teacherUserId);
        Task<int> CreateAsync(Class cls);          // returns new classNo
        Task<bool> UpdateAsync(Class cls);
        Task<bool> DeleteAsync(int classNo);

        // StudentClass (join table) operations — merged here per file map
        Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo);
        Task<IEnumerable<Class>> GetClassesForStudentAsync(int userId);
        Task<bool> EnrollStudentAsync(int userId, int classNo);
        Task<bool> UnenrollStudentAsync(int userId, int classNo);
        Task<bool> IsEnrolledAsync(int userId, int classNo);
    }

    public class ClassRepository : IClassRepository
    {
        // create a constructor that takes in AppDbContext and assigns it to a private readonly field _db

        public Task<IEnumerable<Class>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Class?> GetByIdAsync(int classNo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Class>> GetByTeacherAsync(int teacherUserId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(Class cls)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Class cls)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int classNo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetStudentsInClassAsync(int classNo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Class>> GetClassesForStudentAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EnrollStudentAsync(int userId, int classNo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnenrollStudentAsync(int userId, int classNo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEnrolledAsync(int userId, int classNo)
        {
            throw new NotImplementedException();
        }
    }
}