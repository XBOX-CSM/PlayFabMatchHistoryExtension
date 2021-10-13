using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Util.Repository;

[assembly: FunctionsStartup(typeof(PublicApi.Startup))]
namespace PublicApi
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