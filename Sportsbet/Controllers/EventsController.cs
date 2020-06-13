using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportsbet.BLL.Services;
using Sportsbet.DB;
using Sportsbet.Filters;
using Sportsbet.Model;

namespace Sportsbet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        //private readonly IMapper _mapper;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var list = await _eventService.GetEventsAsync();

            return new List<Event>(list);
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(string id)
        {
            return await _eventService.GetEventAsync(id);
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(string id, Event @event)
        {
            await _eventService.UpdateEventAsync(id, @event);
            return NoContent();
        }

        // POST: api/Events
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
            var created = await _eventService.InsertEventAsync(@event);
            return created;
        }

        [ApiKeyAuth]
        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Event>> DeleteEvent(string id)
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }

        [HttpGet("Save")]
        public void Save()
        {
            _eventService.SaveToJson();
        }

        [HttpGet("Load")]
        public void Load()
        {
            _eventService.LoadFromJson();
        }

    }
}
