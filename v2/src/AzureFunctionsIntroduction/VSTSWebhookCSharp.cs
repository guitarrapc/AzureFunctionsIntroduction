using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Text;

namespace AzureFunctionsIntroduction
{
    public static class VSTSWebhookCSharp
    {
        private static readonly string webhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");

        [FunctionName("VSTSWebhookCSharp")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(VSTSWebhookCSharp)} : Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);
            log.Info(jsonContent);
            var jsonObject = JsonConvert.DeserializeObject<VSTSWebHook>(jsonContent);
            var message = $@"VSTS New Build Executed!
Job Name : {jsonObject.resource.definition.name}
Status : {jsonObject.resource.status}
Detail Message : {jsonObject.detailedMessage.markdown}
";
            log.Info(message);
            var payload = new
            {
                channel = "#github",
                username = "Azure Function Bot",
                text = message,
                icon_url = "https://azure.microsoft.com/svghandler/visual-studio-team-services/?width=300&height=300",
            };

            var jsonString = JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(webhookUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                return req.CreateResponse(res.StatusCode, new
                {
                    body = $"VSTS Build message. Message : {message}",
                });
            }
        }

        public class VSTSWebHook
        {
            public string subscriptionId { get; set; }
            public int notificationId { get; set; }
            public string id { get; set; }
            public string eventType { get; set; }
            public string publisherId { get; set; }
            public Message message { get; set; }
            public Detailedmessage detailedMessage { get; set; }
            public Resource resource { get; set; }
            public string resourceVersion { get; set; }
            public Resourcecontainers resourceContainers { get; set; }
            public DateTime createdDate { get; set; }
        }

        public class Message
        {
            public string text { get; set; }
            public string html { get; set; }
            public string markdown { get; set; }
        }

        public class Detailedmessage
        {
            public string text { get; set; }
            public string html { get; set; }
            public string markdown { get; set; }
        }

        public class Resource
        {
            public int id { get; set; }
            public string status { get; set; }
            public string result { get; set; }
            public DateTime queueTime { get; set; }
            public DateTime startTime { get; set; }
            public DateTime finishTime { get; set; }
            public string url { get; set; }
            public Definition definition { get; set; }
            public string uri { get; set; }
            public string sourceBranch { get; set; }
            public string sourceVersion { get; set; }
            public Queue queue { get; set; }
            public string priority { get; set; }
            public string reason { get; set; }
            public Requestedfor requestedFor { get; set; }
            public Requestedby requestedBy { get; set; }
            public DateTime lastChangedDate { get; set; }
            public Lastchangedby lastChangedBy { get; set; }
            public Orchestrationplan orchestrationPlan { get; set; }
            public Logs logs { get; set; }
            public Repository repository { get; set; }
        }

        public class Definition
        {
            public string type { get; set; }
            public int revision { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public Project project { get; set; }
        }

        public class Project
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string state { get; set; }
        }

        public class Queue
        {
            public object pool { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Requestedfor
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string uniqueName { get; set; }
            public string url { get; set; }
            public string imageUrl { get; set; }
        }

        public class Requestedby
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string uniqueName { get; set; }
            public string url { get; set; }
            public string imageUrl { get; set; }
            public bool isContainer { get; set; }
        }

        public class Lastchangedby
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string uniqueName { get; set; }
            public string url { get; set; }
            public string imageUrl { get; set; }
            public bool isContainer { get; set; }
        }

        public class Orchestrationplan
        {
            public string planId { get; set; }
        }

        public class Logs
        {
            public int id { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Repository
        {
            public string id { get; set; }
            public string type { get; set; }
            public object clean { get; set; }
            public bool checkoutSubmodules { get; set; }
        }

        public class Resourcecontainers
        {
            public Collection collection { get; set; }
            public Account account { get; set; }
            public Project1 project { get; set; }
        }

        public class Collection
        {
            public string id { get; set; }
        }

        public class Account
        {
            public string id { get; set; }
        }

        public class Project1
        {
            public string id { get; set; }
        }
    }
}
