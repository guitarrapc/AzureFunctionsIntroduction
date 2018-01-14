using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Text;

namespace AzureFunctionsIntroduction
{
    public static class GithubWebhookCSharp
    {
        private static readonly string webhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");

        [FunctionName("GithubWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(WebHookType = "github")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("GithubWebhookCSharp : C# HTTP trigger function processed a request.");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            //var comment = (string)data.comment;
            var body = (string)data.comment.body;
            var user = (string)data.sender.login;
            var repository = (string)data.repository.full_name;

            if (data.comment.body == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass comment:body properties in the input object"
                });
            }
            log.Info($"GitHub WebHook triggered!, {data}");
            var message = $@"New GitHub comment posted by {user} at {repository},
Url : {data.comment.url}
Tite : {data.issue.title}
-----
{data.comment.body}";

            var payload = new
            {
                channel = "#github",
                username = "Azure Function Bot",
                text = message,
                icon_url = "https://azure.microsoft.com/svghandler/functions/?width=300&height=300",
            };

            var jsonString = JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(webhookUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                return req.CreateResponse(res.StatusCode, new
                {
                    body = $"From Github: {message}",
                });
            }
        }
    }
}
