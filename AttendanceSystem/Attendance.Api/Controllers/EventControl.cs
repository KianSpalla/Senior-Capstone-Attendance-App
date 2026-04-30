using Attendance.Api.Models;
using Attendance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events.Select(_eventService.ToResponse));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            return evt is null ? NotFound() : Ok(_eventService.ToResponse(evt));
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var evt = await _eventService.GetEventByCodeAsync(code);
            return evt is null ? NotFound() : Ok(_eventService.ToResponse(evt));
        }

        [HttpGet("host/{hostEnum}")]
        public async Task<IActionResult> GetByHost(string hostEnum)
        {
            var events = await _eventService.GetEventsByHostAsync(hostEnum);
            return Ok(events.Select(_eventService.ToResponse));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Event evt)
        {
            var result = await _eventService.CreateEventAsync(evt);

            if (result.Status is not EventSaveStatus.Success || result.EventId is null)
                return ToEventSaveError(result.Status);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.EventId },
                _eventService.ToResponse(evt)
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Event evt)
        {
            evt.eventId = id;

            var result = await _eventService.UpdateEventAsync(evt);
            return result.Status is EventSaveStatus.Success ? NoContent() : ToEventSaveError(result.Status);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventService.DeleteEventAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("{id:int}/report")]
        public async Task<IActionResult> GetAttendanceReport(int id)
        {
            var report = await _eventService.GetAttendanceReportAsync(id);
            return report is null ? NotFound() : Ok(report);
        }

        [HttpGet("code/{code}/qr")]
        public async Task<IActionResult> GetQrCodeByCode(string code)
        {
            var evt = await _eventService.GetEventByCodeAsync(code);

            if (evt is null)
                return NotFound();

            return Content(_eventService.GetQrCodeSvg(evt.eventCode), "image/svg+xml");
        }

        [HttpGet("{id:int}/qr")]
        public async Task<IActionResult> GetQrCodeById(int id)
        {
            var evt = await _eventService.GetEventByIdAsync(id);

            if (evt is null)
                return NotFound();

            return Content(_eventService.GetQrCodeSvg(evt.eventCode), "image/svg+xml");
        }

        private IActionResult ToEventSaveError(EventSaveStatus status)
        {
            return status switch
            {
                EventSaveStatus.NotFound => NotFound(),
                EventSaveStatus.HostNotFound => BadRequest("The event host does not exist."),
                EventSaveStatus.HostNotAllowed => BadRequest("Only admin or teacher users can host events."),
                EventSaveStatus.Invalid => BadRequest("Event name is required and capacity must be greater than zero when provided."),
                _ => BadRequest()
            };
        }
    }
}
