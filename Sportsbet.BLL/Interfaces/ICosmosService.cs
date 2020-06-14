using Sportsbet.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sportsbet.BLL.Services
{
    public interface ICosmosService
    {
        Task<Event> GetEventAsync(string eventId);
        Task<IEnumerable<Event>> GetEventsAsync();
        Task<Event> InsertEventAsync(Event newEvent);
        Task UpdateEventAsync(string eventId, Event updatedEvent);
        Task DeleteEventAsync(string eventId);
        void SaveToJson(string path = "valami");
        void LoadFromJson(string path = "valami");

    }
}
