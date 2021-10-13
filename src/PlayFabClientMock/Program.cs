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

            var pfSettings = new PlayFabConnectionSettings
            {
                TitleId = Environment.GetEnvironmentVariable("pf_titleId"),
                Secret = Environment.GetEnvironmentVariable("pf_secret")
            };

            PlayFabSettings.staticSettings.TitleId = pfSettings.TitleId;
            PlayFabSettings.staticSettings.DeveloperSecretKey = pfSettings.Secret;

            Random random = new Random();

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);
            if (titleEntityResponse.Result != null)
            {
                var authenticationModel = new PlayFab.ClientModels.LoginWithCustomIDRequest { CreateAccount = false, TitleId = pfSettings.TitleId, CustomId = "3CCBFE96B2721B77"};

               var dude =  await PlayFab.PlayFabClientAPI.LoginWithCustomIDAsync(authenticationModel);

                var naste = true;

            }
            else
            {
                Console.WriteLine("broken!!");
            }
        }
    }
}