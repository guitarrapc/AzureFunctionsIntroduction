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
using AzureFunctionsIntroduction.Notify;

namespace AzureFunctionsIntroduction
{
    public static class WebhookCSharpSendToSlack
    {
        private static INotify notify = new NotifySlack();

        [FunctionName("WebhookCSharpSendToSlack")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(WebhookCSharpSendToSlack)} : C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.channel == null || data.username == null || data.text == null || data.icon_url == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass channel/username/text/icon_url properties in the input object"
                });
            }

            var payload = new
            {
                channel = data.channel,
                username = data.username,
                text = data.text,
                icon_url = data.icon_url,
            };
            var json = JsonConvert.SerializeObject(payload);
            var res = await notify.SendAsync(json);

            return req.CreateResponse(res.StatusCode, new
            {
                body = $"Send to Slack for following. text : {payload.text}",
            });
        }
    }
}
