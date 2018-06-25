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
using AzureFunctionsIntroduction.Teams;

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
            if (data.title == null || data.text == null || data.urlname == null || data.url == null)
            {
                postJson = jsonContent;
            }
            else
            {
                /*
                Input JSON Data Format
                {
                    "title": "hogemoge Title",
                    "text": "hogemoge text",
                    "urlname": "hogemo url button name",
                    "url": "https://google.com/"
                }
                */
                postJson = NotifyTeams.ToJson((string)data.title, (string)data.text, (string)data.urlname, (string)data.url);
            };

            var res = await NotifyTeams.SendAsync(postJson);
            return req.CreateResponse(res.StatusCode, new
            {
                body = $"Send to Teams for following. text : {data.text}, response : {res.RequestMessage}",
            });
        }
    }


    public class TeamsMessage
    {
        public string title { get; set; }
        public string text { get; set; }
        public Potentialaction[] potentialAction { get; set; }
    }

    public class Potentialaction
    {
        [JsonProperty(PropertyName = "@context")]
        public string context { get; set; } = "http://schema.org";
        [JsonProperty(PropertyName = "@type")]
        public string type { get; set; } = "ViewAction";
        public string name { get; set; }
        public string[] target { get; set; }
    }

}
