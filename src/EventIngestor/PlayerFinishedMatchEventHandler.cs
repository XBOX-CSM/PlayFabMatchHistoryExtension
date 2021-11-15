using System.Threading.Tasks;
using Util.Model;
using Util.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventIngestor
{
    public class PlayerFinishedMatchEventHandler
    {
        private readonly MatchRepository matchRepository;

        public PlayerFinishedMatchEventHandler(MatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }
        
        [FunctionName("PlayerFinishedMatchEventHandler")]
        public async Task RunAsync([QueueTrigger("player-finished-match-queue", Connection = "EventQueueStorage")] string myQueueItem, ILogger log)
        {
            var context = JsonConvert.DeserializeObject<PlayerPlayStreamFunctionExecutionContext<dynamic>>(myQueueItem);
            var eventData = JsonConvert.DeserializeObject<PlayerFinishedMatchEvent>(context.PlayStreamEventEnvelope.EventData);
            var match = new Match
            {
                Id = eventData.MatchData.MatchId,
                IsMatchWon = eventData.MatchData.IsMatchWon,
                MasterPlayerEntityId = eventData.EntityId
            };
            await this.matchRepository.Save(match);
        }
    }
}