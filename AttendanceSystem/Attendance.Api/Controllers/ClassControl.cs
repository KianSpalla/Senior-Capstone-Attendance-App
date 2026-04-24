//merge student services into this controller/ get https request for both students and classes
using AttendanceApp.Models;
using AttendanceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApp.Controllers
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

        // GET api/class
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }

        // GET api/class/{classNo}
        [HttpGet("{classNo:int}")]
        public async Task<IActionResult> GetById(int classNo)
        {
            var cls = await _classService.GetClassByIdAsync(classNo);
            return cls is null ? NotFound() : Ok(cls);
        }

        // GET api/class/teacher/{teacherId}
        [HttpGet("teacher/{teacherId:int}")]
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            var classes = await _classService.GetClassesByTeacherAsync(teacherId);
            return Ok(classes);
        }

        // POST api/class
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Class cls)
        {
            var newId = await _classService.CreateClassAsync(cls);
            return CreatedAtAction(nameof(GetById), new { classNo = newId }, new { classNo = newId });
        }

        // PUT api/class/{classNo}
        [HttpPut("{classNo:int}")]
        public async Task<IActionResult> Update(int classNo, [FromBody] Class cls)
        {
            cls.ClassNo = classNo;
            var success = await _classService.UpdateClassAsync(cls);
            return success ? NoContent() : NotFound();
        }

        // DELETE api/class/{classNo}
        [HttpDelete("{classNo:int}")]
        public async Task<IActionResult> Delete(int classNo)
        {
            var success = await _classService.DeleteClassAsync(classNo);
            return success ? NoContent() : NotFound();
        }

        // ── StudentClass (enrollment) endpoints ──────────────────────────────

        // GET api/class/{classNo}/students
        [HttpGet("{classNo:int}/students")]
        public async Task<IActionResult> GetStudents(int classNo)
        {
            var students = await _classService.GetStudentsInClassAsync(classNo);
            return Ok(students);
        }

        // GET api/class/student/{userId}
        [HttpGet("student/{userId:int}")]
        public async Task<IActionResult> GetClassesForStudent(int userId)
        {
            var classes = await _classService.GetClassesForStudentAsync(userId);
            return Ok(classes);
        }

        // POST api/class/{classNo}/enroll/{userId}
        [HttpPost("{classNo:int}/enroll/{userId:int}")]
        public async Task<IActionResult> Enroll(int classNo, int userId)
        {
            var success = await _classService.EnrollStudentAsync(userId, classNo);
            if (!success) return Conflict("Student is already enrolled or enrollment failed.");
            return NoContent();
        }

        // DELETE api/class/{classNo}/unenroll/{userId}
        [HttpDelete("{classNo:int}/unenroll/{userId:int}")]
        public async Task<IActionResult> Unenroll(int classNo, int userId)
        {
            var success = await _classService.UnenrollStudentAsync(userId, classNo);
            return success ? NoContent() : NotFound();
        }
    }
}