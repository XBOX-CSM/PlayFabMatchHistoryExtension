
using PlayFab;
using PlayFab.AuthenticationModels;

namespace GameServerMock
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var playerIdList = new[] { "663E07204C3DB17C", "91F18604695EEE58", "8982965E4E8581AC", "89F845F1589FCC45", "89F845F1589FCC45", "69D7719CC4C64C38" };
           
            PlayFab.PlayFabSettings.staticSettings.TitleId = PlayFabSettings.TitleId;
            PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = PlayFabSettings.Secret;

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);
            if (titleEntityResponse.Result != null)
            {
                


            }
            else
            {
                Console.WriteLine("broken!!");
            }
        }
    }
}