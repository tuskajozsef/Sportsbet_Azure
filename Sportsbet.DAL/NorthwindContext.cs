using Microsoft.EntityFrameworkCore;
using Sportsbet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sportsbet.DB
{
    public class NorthwindContext : DbContext
    {

        //public static readonly ILoggerFactory MyLoggerFactory
        //   = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public NorthwindContext() { }

        public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.
                UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=dotnet;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}
