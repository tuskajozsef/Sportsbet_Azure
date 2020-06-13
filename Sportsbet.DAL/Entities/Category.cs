using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sportsbet.Model
{
    public class Category
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Event> Events { get; } = new List<Event>();

        public Category()
        {
            Id = Guid.NewGuid();
        }

    }
}
