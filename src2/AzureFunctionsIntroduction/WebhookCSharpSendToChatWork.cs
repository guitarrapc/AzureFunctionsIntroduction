using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using Chatwork.Service;

namespace AzureFunctionsIntroduction
{
    public static class WebhookCSharpSendToChatWork
    {
        private static readonly string chatworkApiKey = Environment.GetEnvironmentVariable("ChatworkApiKey");

        [FunctionName("WebhookCSharpSendToChatWork")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"WebhookCSharpSendToChatWork : Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.channel == 0 || data.text == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass channel/text properties in the input object"
                });
            }

            int roomId = data.channel;
            string body = data.text;

            var client = new ChatworkClient(chatworkApiKey);
            await client.Room.SendMessgesAsync(roomId, body);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                RoomId = roomId,
                Message = body,
            });
        }
    }
}
