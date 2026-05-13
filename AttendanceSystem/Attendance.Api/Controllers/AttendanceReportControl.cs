using Attendance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendanceReportService;

        public AttendanceReportController(IAttendanceReportService attendanceReportService)
        {
            _attendanceReportService = attendanceReportService;
        }

        [HttpGet("student-progress")]
        public async Task<IActionResult> GetStudentProgress(
            [FromQuery] int? classNo,
            [FromQuery] int? eventId,
            [FromQuery] string? status)
        {
            var report = await _attendanceReportService.GetStudentProgressAsync(classNo, eventId, status);
            return Ok(report);
        }
    }
}
