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
    public class CosmosService : ICosmosService
    {
        private Container eventContainer;
        private Container categoryContainer;
        private Container ticketContainer;

        public CosmosService(CosmosClient dbClient, string databaseName, string containerName)
        {
            eventContainer = dbClient.GetContainer(databaseName, "events");
            ticketContainer = dbClient.GetContainer(databaseName, "tickets");
            categoryContainer = dbClient.GetContainer(databaseName, "categories");

            SeedDatabaseAsync();
        }

        private async void SeedDatabaseAsync()
        {
            if (!eventContainer.GetItemLinqQueryable<Event>(true).ToList().Any())
            {
                var category_1 = new Category
                {
                    Name = "Football",
            };

                var category_2 = new Category
                {
                    Name = "Handball",
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

                await eventContainer.CreateItemAsync(event_1);
                await eventContainer.CreateItemAsync(event_2);
                await eventContainer.CreateItemAsync(event_3);
  
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
            await eventContainer.DeleteItemAsync<Event>(eventId, new PartitionKey(eventId));
        }

        public async Task<Event> GetEventAsync(string eventId)
        {
            try
            {
                ItemResponse<Event> response = await eventContainer.ReadItemAsync<Event>(eventId.ToString(), new PartitionKey());
                return response.Resource;
            }

            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new Event();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            var query = eventContainer.GetItemLinqQueryable<Event>(true);
            List<Event> events = new List<Event>(query.ToList());

            return events;
           
        }

        public async Task<Event> InsertEventAsync(Event newEvent)
        {
            return await eventContainer.CreateItemAsync(newEvent);
        }

        public async Task UpdateEventAsync(string eventId, Event updatedEvent)
        {
            await eventContainer.UpsertItemAsync<Event>(updatedEvent, new PartitionKey(eventId));
        }

        public async void SaveToJson(string path)
        {
            var events = await eventContainer.GetItemLinqQueryable<Event>()
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
                        await eventContainer.CreateItemAsync<Event>(e);
                }
            }

            catch (Exception e)
            {
                if (e is IOException) { }
            }

        }
    }
}
