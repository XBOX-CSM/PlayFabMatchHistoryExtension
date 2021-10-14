using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AuthenticationModels;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Util.Model;
using Util.Repository;

namespace PublicApi
{
    public class GetPlayerMatchHistory
    {
        private readonly MatchRepository cosmosClient;

        public GetPlayerMatchHistory(MatchRepository repository)
        {
            this.cosmosClient = repository;
        }

        [FunctionName("GetPlayerMatchHistory")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "GetPlayerMatchHistory" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "PlayFab SessionTicket", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **PlayFabSessionTicket** is needed in order to perform operations on this API")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<PlayerAuthentication>(requestBody);

            PlayFab.PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable("PlayFabTitleId");
            PlayFab.PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable("PlayFabDeveloperSecret");

            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);

            if (titleEntityResponse.Result != null)
            {
                var request = new PlayFab.ServerModels.AuthenticateSessionTicketRequest
                {
                    SessionTicket = data.SessionTicket
                };

                var response = await PlayFabServerAPI.AuthenticateSessionTicketAsync(request);

                var userId = response.Result.UserInfo.PlayFabId;

                var result = await cosmosClient.Get(userId);

                return new OkObjectResult(JsonConvert.SerializeObject(result));
            }
            else
            {
                log.LogError("Couldn't validate Session Ticket with PlayFab!" + titleEntityResponse.Error);
            }

            return new NotFoundObjectResult("No HistoryFound");
        }

    }
}

