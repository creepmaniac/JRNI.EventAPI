using JRNI.EventAPI.Interface;
using JRNI.EventAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace JRNI.EventAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventApiService _eventApiService;

        public EventsController(IEventApiService eventApiService)
        {
            _eventApiService = eventApiService ?? throw new ArgumentNullException(nameof(eventApiService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsAsync([FromQuery] string email)
        {
            return await _eventApiService.GetFutureEventsAsync(email);
        }
    }
}
