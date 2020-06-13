using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sportsbet.Model
{
    public class Ticket
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        //public double AllOdds 
        //{
        //    //get
        //    //{
        //    //    double odds = 1;
        //    //    foreach (Event e in Events)
        //    //        odds *= Choices[e];

        //    //    return odds;
        //    //}
        //}
        public int Stake { get; set; }
        public int ExpectedWin { get; set; }
        public List<Event> Events { get; set; }
            = new List<Event>();
        
        public Ticket()
        {
            Id = Guid.NewGuid();
        }
    }
}
