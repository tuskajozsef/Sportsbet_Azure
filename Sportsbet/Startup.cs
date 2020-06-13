using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sportsbet.BLL.Services;
using Sportsbet.DB;
using Sportsbet.Model;

namespace Sportsbet
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddDbContext<NorthwindContext>(o =>
            //o.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly("Sportsbet")));
            //services.AddTransient<IEventService, EventService>();

            services.AddSingleton <IEventService>(InitializeCosmosClientInstanseAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }

        private static async Task<EventService> InitializeCosmosClientInstanseAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            //string containerName = configurationSection.GetSection("DatabaseName").Value;
            string account = configurationSection.GetSection("Endpoint").Value;
            string key = configurationSection.GetSection("Key").Value;

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            CosmosClient client = clientBuilder
                .WithConnectionModeDirect()
                .Build();
            EventService eventService = new EventService(client, databaseName, "events");
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync("events", "/id");
            await database.Database.CreateContainerIfNotExistsAsync("tickets", "/id");
            await database.Database.CreateContainerIfNotExistsAsync("categories", "/id");

            return eventService;
                

        }

    }
}
