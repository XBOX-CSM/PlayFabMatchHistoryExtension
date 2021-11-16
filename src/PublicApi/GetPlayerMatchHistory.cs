using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlayFab;
using PlayFab.AuthenticationModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Util.Model;
using Util.Repository;

namespace PublicApi
{
    public class GetPlayerMatchHistory
    {
        private readonly MatchRepository matchRepositoy;

        public GetPlayerMatchHistory(MatchRepository repository)
        {
            this.matchRepositoy = repository;
        }

        [FunctionName("GetPlayerMatchHistory")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "GetPlayerMatchHistory" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "SessionTicket", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **PlayFabSessionTicket** is needed in order to perform operations on this API")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string sessionTicket = req.Query["SessionTicket"].ToString();   // Get the SessionTicket from the Query
            
            // PlayFab API Auth - use environment variables
            PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable("TF_VAR_pf_title_id");
            PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable("TF_VAR_pf_developer_secret");

            // Get the TitleEntity
            var getTitleEntityTokenRequest = new GetEntityTokenRequest(); //Do not need to set Entity
            var titleEntityResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(getTitleEntityTokenRequest);

            if (titleEntityResponse.Result != null)
            {
                // Authenticate the user using the SessionTicket
                var request = new PlayFab.ServerModels.AuthenticateSessionTicketRequest
                {
                    SessionTicket = sessionTicket
                };
                var response = await PlayFabServerAPI.AuthenticateSessionTicketAsync(request);
                if (response.Error != null)
                {
                    // Forward the error returned by PlayFab
                    var errorResponse = new ObjectResult(response.Error);
                    errorResponse.StatusCode = response.Error.HttpCode;
                    return errorResponse;
                }

                var userId = response.Result.UserInfo.PlayFabId;

                List<Match> result = await matchRepositoy.Get(userId);

                return new OkObjectResult(result);
            }
            else
            {
                log.LogError("Couldn't validate Session Ticket with PlayFab!" + titleEntityResponse.Error); // TODO: Remove - not needed
            }

            return new NotFoundObjectResult("No HistoryFound");
        }

    }
}

