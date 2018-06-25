using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzureFunctionsIntroduction
{
    public static class WebhookCSharpSendToTeams
    {
        private static string teamsWebhookUrl = Environment.GetEnvironmentVariable("TeamsIncomingWebhookUrl");

        [FunctionName("WebhookCSharpSendToTeams")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(WebhookCSharpSendToTeams)} : C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            string postJson;
            if (data != null)
            {
                postJson = jsonContent;
                using (var client = new HttpClient())
                {
                    var stringContent = new StringContent(postJson);
                    var res = await client.PostAsync(teamsWebhookUrl, stringContent);
                    return req.CreateResponse(res.StatusCode, new
                    {
                        body = $"Send to Teams for following. text : {data.text}",
                    });
                }
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    body = $"Bad request.",
                });
            }
        }
    }
}
