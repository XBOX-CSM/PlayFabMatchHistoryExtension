using System.Threading.Tasks;
using System;
using EventIngestor.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventIngestor
{
    public class PlayerFinishedMatchEvent
    {
        private readonly MatchRepository matchRepository;

        public PlayerFinishedMatchEvent(MatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }
        
        [FunctionName("PlayerFinishedMatchEvent")]
        public async Task RunAsync([QueueTrigger("player-finished-match-queue", Connection = "EventQueueStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            
            var context = JsonConvert.DeserializeObject<PlayerPlayStreamFunctionExecutionContext<dynamic>>(myQueueItem);
            // context.PlayStreamEventEnvelope.EventData
        }
    }
}