using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;

namespace GithubWebhookCSharp
{
    public class FunctionTrigger
    {
        private static readonly string webhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            var jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

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
                    body = $"GithubWebhookCSharp message. Message : {message}",
                });
            }
        }
    }
}