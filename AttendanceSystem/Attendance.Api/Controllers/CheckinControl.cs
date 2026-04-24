using AttendanceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        // GET api/checkin/event/{eventId}
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var checkIns = await _checkInService.GetCheckInsForEventAsync(eventId);
            return Ok(checkIns);
        }

        // GET api/checkin/event/{eventId}/count
        [HttpGet("event/{eventId:int}/count")]
        public async Task<IActionResult> GetCount(int eventId)
        {
            var count = await _checkInService.GetCheckInCountAsync(eventId);
            return Ok(new { eventId, count });
        }

        // GET api/checkin/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var checkIns = await _checkInService.GetCheckInsForUserAsync(userId);
            return Ok(checkIns);
        }

        // POST api/checkin
        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var newId = await _checkInService.CheckInAsync(request.EventId, request.UserId);
            if (newId == -1)
                return Conflict("Already checked in or event is at capacity.");
            return CreatedAtAction(null, new { checkInId = newId });
        }

        // DELETE api/checkin/{checkInId}
        [HttpDelete("{checkInId:int}")]
        public async Task<IActionResult> Delete(int checkInId)
        {
            var success = await _checkInService.RemoveCheckInAsync(checkInId);
            return success ? NoContent() : NotFound();
        }
    }

    public record CheckInRequest(int EventId, int UserId);
}