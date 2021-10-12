using Bogus;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AuthenticationModels;

namespace GameServerMock
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var playerIdList = new[] { "663E07204C3DB17C" };
            var matchIdList = new[] { "0000" };

            var testGameServerEvent = new Faker<StatusUpdateEntity>().StrictMode(true)
                .RuleFor(s => s.MatchId, f => f.PickRandom(matchIdList))
                .RuleFor(s => s.IsMatchWon, f => f.Random.Bool());

            var testEvent = testGameServerEvent.Generate();
            var eventAsJson = JsonConvert.SerializeObject(testEvent);

            PlayFab.PlayFabSettings.staticSettings.TitleId = PlayFabSettings.TitleId;
            PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = PlayFabSettings.Secret;

            Random random = new Random();

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);
            if (titleEntityResponse.Result != null)
            {
                var writeEventRequest = new PlayFab.ServerModels.WriteServerPlayerEventRequest
                {
                    EventName = "player_finished_match",
                    PlayFabId = playerIdList[random.Next(0, playerIdList.Length)],
                    Timestamp = DateTime.Now,
                    Body = new Dictionary<string, object>() { { "test", testEvent } },
                };

                var test = await PlayFabServerAPI.WritePlayerEventAsync(writeEventRequest);

                Console.WriteLine("Succes!!");
            }
            else
            {
                Console.WriteLine("broken!!");
            }
        }
    }
}