using Attendance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckinController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckinController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var checkins = await _checkInService.GetCheckInsForEventAsync(eventId);
            return Ok(checkins);
        }

        [HttpGet("event/{eventId:int}/count")]
        public async Task<IActionResult> GetCount(int eventId)
        {
            var count = await _checkInService.GetCheckInCountAsync(eventId);
            return Ok(new { eventId, count });
        }

        [HttpGet("user/{studentEnum}")]
        public async Task<IActionResult> GetByUser(string studentEnum)
        {
            var checkins = await _checkInService.GetCheckInsForUserAsync(studentEnum);
            return Ok(checkins);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckinRequest request)
        {
            var result = await _checkInService.CheckInAsync(request.eventId, request.Enum);

            if (result.Status is not CheckInStatus.Success || result.CheckInId is null)
                return ToCheckInError(result.Status);

            return Created("", new { checkInId = result.CheckInId });
        }

        [HttpPost("code/{eventCode}")]
        public async Task<IActionResult> CheckInByEventCode(string eventCode, [FromBody] CheckinByCodeRequest request)
        {
            var result = await _checkInService.CheckInByEventCodeAsync(eventCode, request.Enum);

            if (result.Status is not CheckInStatus.Success || result.CheckInId is null)
                return ToCheckInError(result.Status);

            return Created("", new { checkInId = result.CheckInId });
        }

        [HttpDelete("{checkInId:int}")]
        public async Task<IActionResult> Delete(int checkInId)
        {
            var success = await _checkInService.RemoveCheckInAsync(checkInId);
            return success ? NoContent() : NotFound();
        }

        private IActionResult ToCheckInError(CheckInStatus status)
        {
            return status switch
            {
                CheckInStatus.EventNotFound => NotFound("Event not found."),
                CheckInStatus.UserNotFound => NotFound("User not found."),
                CheckInStatus.UserNotAllowed => BadRequest("Only student users can check in to events."),
                CheckInStatus.AlreadyCheckedIn => Conflict("Student is already checked in for this event."),
                CheckInStatus.EventAtCapacity => Conflict("Event is at capacity."),
                _ => BadRequest()
            };
        }
    }

    public record CheckinRequest(int eventId, string Enum);

    public record CheckinByCodeRequest(string Enum);
}
