using Bogus;
using PlayFab;
using PlayFab.AuthenticationModels;

namespace GameServerMock
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // List of PlayerIds and mock MatchIds to pick from for the example data
            var playerIdList = new[] { "663E07204C3DB17C", "91F18604695EEE58", "8982965E4E8581AC", "89F845F1589FCC45", "89F845F1589FCC45", "69D7719CC4C64C38" };

            // PlayFab API authentication credentials
            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable("TF_VAR_pf_title_id");
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable("TF_VAR_pf_developer_secret");

            Random random = new Random();

            // Check if Title exists
            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);
            if (titleEntityResponse.Result != null)
            {
                while (true)
                {
                    StatusUpdateEntity matchResult = GenerateRandomMatchResult();
                    
                    var writeEventRequest = new PlayFab.ServerModels.WriteServerPlayerEventRequest
                    {
                        EventName = "player_finished_match",
                        PlayFabId = playerIdList[random.Next(0, playerIdList.Length)],
                        Timestamp = DateTime.Now,
                        Body = new Dictionary<string, object>() { { "MatchData", matchResult } },
                    };

                    await PlayFabServerAPI.WritePlayerEventAsync(writeEventRequest);

                    Console.WriteLine($"Event Send for Player {writeEventRequest.PlayFabId} and Match {matchResult.MatchId}");
                    Thread.Sleep(1000); // Sleep a bit to avoid sending too many test events
                }
            }
            else
            {
                Console.WriteLine("broken!!");
            }
        }

        /// <summary>
        /// Generate a random match result
        /// </summary>
        /// <returns>A random match result</returns>
        private static StatusUpdateEntity GenerateRandomMatchResult()
        {
            var testGameServerEvent = new Faker<StatusUpdateEntity>().StrictMode(true)
                .RuleFor(s => s.MatchId, f => Guid.NewGuid().ToString())
                .RuleFor(s => s.IsMatchWon, f => f.Random.Bool());
            var testEvent = testGameServerEvent.Generate();
            return testEvent;
        }
    }
}