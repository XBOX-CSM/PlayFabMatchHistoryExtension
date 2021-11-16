using PlayFab;
using PlayFab.AuthenticationModels;
using System.Net;
using System.Text.Json;
using Util;
using Util.Model;

namespace PlayFabClientMock
{
    public class Program
    {
        private const string CustomBackendUrl = "http://localhost:7071/api/GetPlayerMatchHistory";

        public static async Task Main(string[] args)
        {
            var playerIdList = new[] {
                new PlayFabUser
                {
                    TitleId = "889297498ED8CCD2",
                    MasterId = "663E07204C3DB17C",
                    CustomId = "3CCBFE96B2721B77"

                },
                new PlayFabUser
                {
                    TitleId = "8517B349A43BB550",
                    MasterId = "69D7719CC4C64C38",
                    CustomId = "D5908C8271666CD"

                },
                new PlayFabUser
                {
                    TitleId = "3B1D8048824239A4",
                    MasterId = "89F845F1589FCC45",
                    CustomId = "D79728A4FA28AD53"

                },
                new PlayFabUser
                {
                    TitleId = "79AB05E301867A9E",
                    MasterId = "8982965E4E8581AC",
                    CustomId = "88591B580812F6F7"

                },
                new PlayFabUser
                {
                    TitleId = "5FA4982A6F7E2E3",
                    MasterId = "91F18604695EEE58",
                    CustomId = "47656583241B010B"

                }
            };

            var pfSettings = new PlayFabConnectionSettings
            {
                TitleId = Environment.GetEnvironmentVariable("TF_VAR_pf_title_id"),
                Secret = Environment.GetEnvironmentVariable("TF_VAR_pf_developer_secret")
            };

            PlayFabSettings.staticSettings.TitleId = pfSettings.TitleId;
            PlayFabSettings.staticSettings.DeveloperSecretKey = pfSettings.Secret;

            Random random = new Random();

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityTokenResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);

            if (titleEntityTokenResponse.Result != null)
            {
                var client = new HttpClient();
                while (true)
                {
                    var randPlayer = random.Next(0, playerIdList.Length);
                    var authenticationModel = new PlayFab.ClientModels.LoginWithCustomIDRequest {
                        CreateAccount = false,
                        TitleId = pfSettings.TitleId,
                        CustomId = playerIdList[randPlayer].CustomId
                    };

                    var loginResponse = await PlayFabClientAPI.LoginWithCustomIDAsync(authenticationModel);

                    var content = new StringContent(JsonSerializer.Serialize(new PlayerAuthentication
                    {
                        SessionTicket = loginResponse.Result.SessionTicket,
                    }));

                    try
                    {
                        string requestUri = $"{CustomBackendUrl}?SessionTicket={WebUtility.UrlEncode(loginResponse.Result.SessionTicket)}";
                        var playerMatchHistoryResponse = await client.GetAsync(requestUri);
                        List<Match>? matchList = JsonSerializer.Deserialize<List<Match>>(playerMatchHistoryResponse.Content.ReadAsStream());
                        if (matchList is null)
                        {
                            throw new ArgumentNullException(nameof(matchList));
                        }

                        Console.WriteLine("Games Played by Player: " + randPlayer);

                        foreach(Match match in matchList)
                        {
                            Console.WriteLine("... " + match.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occured: {ex.Message}");
                    }

                    Thread.Sleep(2000);
                }
            }
            else
            {
                Console.WriteLine("Colund not retrieve Title Entity Token");
            }
        }
    }
}