using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sportsbet.DB;
using Sportsbet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;


namespace Sportsbet.BLL.Services
{
    public class EventService : IEventService
    {
        private Container _container;
        private Container categoryContainer;
        private Container ticketContainer;
        //private readonly NorthwindContext _container;

        public EventService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, "events");
            ticketContainer = dbClient.GetContainer(databaseName, "tickets");
            categoryContainer = dbClient.GetContainer(databaseName, "categories");

            SeedDatabaseAsync();
        }

        private async void SeedDatabaseAsync()
        {
            if (!_container.GetItemLinqQueryable<Event>(true).ToList().Any())
            {
                var category_1 = new Category
                {
                    Name = "Football",
            };

                var category_2 = new Category
                {
                    Name = "Handball",
                    //Id = Guid.NewGuid()
                };

                await categoryContainer.CreateItemAsync(category_1);
                await categoryContainer.CreateItemAsync(category_2);

                var event_1 = new Event
                {
                    HomeTeam = "Köln",
                    AwayTeam = "Mainz",
                    HomeOdds = 2.3,
                    DrawOdds = 3.0,
                    AwayOdds = 2.8,
                    Time = new DateTime(2020, 05, 17, 15, 30, 0),
                    Category = category_1,
                };

                var event_2 = new Event
                {
                    HomeTeam = "Union Berlin",
                    AwayTeam = "Bayern München",
                    HomeOdds = 9.0,
                    DrawOdds = 4.5,
                    AwayOdds = 1.45,
                    Time = new DateTime(2020, 05, 18, 18, 30, 0),
                    Category = category_1,
                };

                var event_3 = new Event
                {
                    HomeTeam = "Köln",
                    AwayTeam = "Mainz",
                    HomeOdds = 2.3,
                    DrawOdds = 3.0,
                    AwayOdds = 2.8,
                    Time = new DateTime(2020, 05, 18, 18, 30, 0),
                    Category = category_2,
                };

                await _container.CreateItemAsync(event_1);
                await _container.CreateItemAsync(event_2);
                await _container.CreateItemAsync(event_3);
  
                Ticket t1 = new Ticket();
                Ticket t2 = new Ticket();

                t1.Events.Add(event_1);
                t1.Events.Add(event_2);

                t2.Events.Add(event_2);
                t2.Events.Add(event_3);

                await ticketContainer.CreateItemAsync(t1);
                await ticketContainer.CreateItemAsync(t2);
            }
        }

        public async Task DeleteEventAsync(string eventId)
        {
            //_container.Events.Remove(new Event { Id = eventId });
            //try
            //{
            //    await _container.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if ((await _container.Events
            //    .SingleOrDefaultAsync(e => e.Id == eventId)) == null)
            //        throw new Exception("Nem található a termék");
            //    else throw;
            //}
            //TODO lehet nem lesz jó
            await _container.DeleteItemAsync<Event>(eventId, new PartitionKey(eventId));
        }

        public async Task<Event> GetEventAsync(string eventId)
        {
            //{
            //    return (await _container.Events
            //       .Include(e => e.HomeTeam)
            //       .Include(e => e.AwayTeam)
            //       .SingleOrDefaultAsync(e => e.Id == eventId))
            //       ?? throw new Exception("Nem található a termék");

            try
            {
                ItemResponse<Event> response = await _container.ReadItemAsync<Event>(eventId.ToString(), new PartitionKey());
                return response.Resource;
            }

            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new Event();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            //var events = await _container.Events
            //    .ToListAsync();

            //return events;

            var query = _container.GetItemLinqQueryable<Event>(true);
            List<Event> events = new List<Event>(query.ToList());

            //while (query.HasMoreResults)
            //{
            //    var response = query.ReadNextAsync();
            //    events.AddRange(response.Result.);
            //}

            return events;
           
        }

        public async Task<Event> InsertEventAsync(Event newEvent)
        {
            //_container.Events.Add(newEvent);
            //await _container.SaveChangesAsync();
            //return newEvent;

            return await _container.CreateItemAsync(newEvent);
        }

        public async Task UpdateEventAsync(string eventId, Event updatedEvent)
        {
            //updatedEvent.Id = eventId;
            //var entry = _container.Attach(updatedEvent);
            //entry.State = EntityState.Modified;
            //try
            //{
            //    await _container.SaveChangesAsync(); //async változat hívása
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if ((await _container.Events
            //            .SingleOrDefaultAsync(p => p.Id == eventId)) == null)
            //        throw new Exception("Nem található a termék");
            //    else throw;
            //}

            await _container.UpsertItemAsync<Event>(updatedEvent, new PartitionKey(eventId));
        }

        public async void SaveToJson(string path)
        {
            var events = await _container.GetItemLinqQueryable<Event>()
                .ToListAsync();

            string file = path;

            try
            {
                using (StreamWriter sw = File.CreateText(file))
                {
                    JsonSerializer js = new JsonSerializer();
                    js.Serialize(sw, events.ToList());
                }
            }

            catch (Exception e)
            {
                if (e is IOException) { }

            }
        }

        public async void LoadFromJson(string path)
        {
            string json = "";
            try
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    var list = new List<Event>();

                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null) break;
                        json += line;

                    }

                    list = JsonConvert.DeserializeObject<List<Event>>(json);

                    foreach (Event e in list)
                        await _container.CreateItemAsync<Event>(e);
                }
            }

            catch (Exception e)
            {
                if (e is IOException) { }
            }

        }
    }
}
