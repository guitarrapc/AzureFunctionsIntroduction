using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace WebhookCSharpSendToSlack
{
    public class FunctionTrigger
    {
        private static string _slackWebhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");

        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

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
            var jsonString = JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(_slackWebhookUrl, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("payload", jsonString)
                }));
                return req.CreateResponse(res.StatusCode, new
                {
                    body = $"Send to Slack for following. text : {data.text}",
                });
            }
        }
    }
}