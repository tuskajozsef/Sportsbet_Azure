using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sportsbet.Model
{
    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime Time { get; set; }
        public double HomeOdds { get; set; }
        public double DrawOdds { get; set; }
        public double AwayOdds { get; set; }

        public Category Category { get; set; }

        public Event()
        {
            Id = Guid.NewGuid();
        }

    }
}
