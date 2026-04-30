using Attendance.Api.Models;
using Attendance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }

        [HttpGet("{classNo:int}")]
        public async Task<IActionResult> GetById(int classNo)
        {
            var cls = await _classService.GetClassByIdAsync(classNo);
            return cls is null ? NotFound() : Ok(cls);
        }

        [HttpGet("teacher/{teacherEnum}")]
        public async Task<IActionResult> GetByTeacher(string teacherEnum)
        {
            var classes = await _classService.GetClassesByTeacherAsync(teacherEnum);
            return Ok(classes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Class cls)
        {
            var result = await _classService.CreateClassAsync(cls);

            if (result.Status is not ClassSaveStatus.Success || result.ClassNo is null)
                return ToClassSaveError(result.Status);

            return CreatedAtAction(
                nameof(GetById),
                new { classNo = result.ClassNo },
                new { classNo = result.ClassNo }
            );
        }

        [HttpPut("{classNo:int}")]
        public async Task<IActionResult> Update(int classNo, [FromBody] Class cls)
        {
            cls.classNo = classNo;

            var result = await _classService.UpdateClassAsync(cls);
            return result.Status is ClassSaveStatus.Success ? NoContent() : ToClassSaveError(result.Status);
        }

        [HttpDelete("{classNo:int}")]
        public async Task<IActionResult> Delete(int classNo)
        {
            var success = await _classService.DeleteClassAsync(classNo);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("{classNo:int}/students")]
        public async Task<IActionResult> GetStudents(int classNo)
        {
            var students = await _classService.GetStudentsInClassAsync(classNo);
            return Ok(students.Select(ToUserResponse));
        }

        [HttpGet("student/{studentEnum}")]
        public async Task<IActionResult> GetClassesForStudent(string studentEnum)
        {
            var classes = await _classService.GetClassesForStudentAsync(studentEnum);
            return Ok(classes);
        }

        [HttpPost("{classNo:int}/enroll/{studentEnum}")]
        public async Task<IActionResult> Enroll(int classNo, string studentEnum)
        {
            var result = await _classService.EnrollStudentAsync(studentEnum, classNo);

            if (result.Status is not EnrollmentStatus.Success)
                return ToEnrollmentError(result.Status);

            return NoContent();
        }

        [HttpDelete("{classNo:int}/unenroll/{studentEnum}")]
        public async Task<IActionResult> Unenroll(int classNo, string studentEnum)
        {
            var success = await _classService.UnenrollStudentAsync(studentEnum, classNo);
            return success ? NoContent() : NotFound();
        }

        private IActionResult ToClassSaveError(ClassSaveStatus status)
        {
            return status switch
            {
                ClassSaveStatus.NotFound => NotFound(),
                ClassSaveStatus.TeacherNotFound => BadRequest("The class teacher does not exist."),
                ClassSaveStatus.TeacherNotAllowed => BadRequest("Only admin or teacher users can teach classes."),
                ClassSaveStatus.Invalid => BadRequest("Class name is required."),
                _ => BadRequest()
            };
        }

        private IActionResult ToEnrollmentError(EnrollmentStatus status)
        {
            return status switch
            {
                EnrollmentStatus.ClassNotFound => NotFound("Class not found."),
                EnrollmentStatus.UserNotFound => NotFound("User not found."),
                EnrollmentStatus.UserNotStudent => BadRequest("Only student users can be enrolled in classes."),
                EnrollmentStatus.AlreadyEnrolled => Conflict("Student is already enrolled in this class."),
                _ => BadRequest()
            };
        }

        private static UserResponse ToUserResponse(User user)
        {
            return new UserResponse(
                user.Enum,
                user.fname,
                user.lname,
                user.email,
                user.phoneNum,
                user.Role,
                user.createdAt
            );
        }
    }
}
