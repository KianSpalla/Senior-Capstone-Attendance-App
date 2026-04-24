using AttendanceApp.Models;
using AttendanceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApp.Controllers
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

        // GET api/event
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // GET api/event/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            return evt is null ? NotFound() : Ok(evt);
        }

        // GET api/event/code/{code}
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var evt = await _eventService.GetEventByCodeAsync(code);
            return evt is null ? NotFound() : Ok(evt);
        }

        // GET api/event/host/{hostId}
        [HttpGet("host/{hostId:int}")]
        public async Task<IActionResult> GetByHost(int hostId)
        {
            var events = await _eventService.GetEventsByHostAsync(hostId);
            return Ok(events);
        }

        // POST api/event
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Event evt)
        {
            var newId = await _eventService.CreateEventAsync(evt);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { eventId = newId });
        }

        // PUT api/event/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Event evt)
        {
            evt.EventId = id;
            var success = await _eventService.UpdateEventAsync(evt);
            return success ? NoContent() : NotFound();
        }

        // DELETE api/event/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventService.DeleteEventAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}