using Bogus;
using PlayFab;
using PlayFab.AuthenticationModels;
using Util;

namespace GameServerMock
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var playerIdList = new[] { "663E07204C3DB17C", "91F18604695EEE58", "8982965E4E8581AC", "89F845F1589FCC45", "89F845F1589FCC45", "69D7719CC4C64C38" };
            var matchIdList = new[] { "7b92c513-3659-4f46-be63-f2d823c19db7", "c713d080-ac7c-46c5-a119-f6004b9b72e3", "0c24427f-2ebf-44a6-8f12-37259924c7e2" };

            var pfSettings = new PlayFabConnectionSettings
            {
                TitleId = Environment.GetEnvironmentVariable("TF_VAR_pf_title_id"),
                Secret = Environment.GetEnvironmentVariable("TF_VAR_pf_developer_secret")
            };

            PlayFabSettings.staticSettings.TitleId = pfSettings.TitleId;
            PlayFabSettings.staticSettings.DeveloperSecretKey = pfSettings.Secret;

            Random random = new Random();

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);
            if (titleEntityResponse.Result != null)
            {
                while (true)
                {
                    var testGameServerEvent = new Faker<StatusUpdateEntity>().StrictMode(true)
                        .RuleFor(s => s.MatchId, f => f.Random.Replace("********-****-****-****-************"))
                        .RuleFor(s => s.IsMatchWon, f => f.Random.Bool());

                    var testEvent = testGameServerEvent.Generate();

                    var writeEventRequest = new PlayFab.ServerModels.WriteServerPlayerEventRequest
                    {
                        EventName = "player_finished_match",
                        PlayFabId = playerIdList[random.Next(0, playerIdList.Length)],
                        Timestamp = DateTime.Now,
                        Body = new Dictionary<string, object>() { { "MatchData", testEvent } },
                    };

                    await PlayFabServerAPI.WritePlayerEventAsync(writeEventRequest);

                    Console.WriteLine("Event Send for Player " + writeEventRequest.PlayFabId + " and Match " + testEvent.MatchId);


                    Thread.Sleep(1000);
                }


            }
            else
            {
                Console.WriteLine("broken!!");
            }
        }
    }
}