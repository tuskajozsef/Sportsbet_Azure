using Newtonsoft.Json;
using Sportsbet.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sportsbet.Client
{
    public static class Client
    {
        private static readonly HttpClient client = new HttpClient();
        static Uri uri = new Uri($"http://dotnethfappservice.azurewebsites.net/api/events");


        public static async Task<List<Event>> LoadEventsAsync()
        {
            try
            {
                string response = await client.GetStringAsync(uri);

                var result = JsonConvert.DeserializeObject<List<Event>>(response);
                return result;
            }

            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public static async Task AddEvent(Event e)
        {
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Post;
            req.RequestUri = uri;
            var httpContent = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");
            req.Content = httpContent;

            var response = await client.SendAsync(req);
        }

        public static async Task<HttpResponseMessage> DeleteEvent(string id)
        {
            HttpRequestMessage req = new HttpRequestMessage();
            Uri deleteUri = new Uri($"{uri.ToString()}/{id}");

            req.Headers.Add("ApiKey", "MyApiKey");
            req.Method = HttpMethod.Delete;
            req.RequestUri = deleteUri;

            return await client.SendAsync(req);
        }

        public static async Task UpdateEvent(Event updatedEvent, string id)
        {
            Uri updateUri = new Uri($"{uri.ToString()}/{id}");

            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Put;
            req.RequestUri = updateUri;
            var httpContent = new StringContent(JsonConvert.SerializeObject(updatedEvent), Encoding.UTF8, "application/json");
            req.Headers.Add("x-ms-documentdb-partitionkey", id);
            req.Content = httpContent;

            var response = await client.SendAsync(req);
        }
    }
}
