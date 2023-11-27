using JRNI.EventAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace JRNI.EventAPI.Interface
{
    public interface IEventApiService
    {
        Task<ActionResult<IEnumerable<Event>>> GetFutureEventsAsync(string email);
    }
}