using System;
using EventIngestor.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(EventIngestor.Startup))]
namespace EventIngestor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("CosmosDb");
            builder.Services.AddSingleton(s => new MatchRepository(connectionString));
        }
    }
}