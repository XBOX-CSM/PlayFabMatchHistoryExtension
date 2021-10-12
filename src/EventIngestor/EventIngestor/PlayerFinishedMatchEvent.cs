using System.Threading.Tasks;
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EventIngestor
{
    public static class PlayerFinishedMatchEvent
    {
        [FunctionName("PlayerFinishedMatchEvent")]
        public static async Task RunAsync([QueueTrigger("player-finished-match-queue", Connection = "EventQueueStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            
        }
    }
}