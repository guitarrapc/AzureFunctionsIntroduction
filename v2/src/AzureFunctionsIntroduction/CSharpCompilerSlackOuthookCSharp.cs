using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Linq;
using System.Text;
using AzureFunctionsIntroduction.Roslyn;

namespace AzureFunctionsIntroduction
{
    public static class CSharpCompilerSlackOuthookCSharp
    {
        private const string TRIGGER_WORD = "@C#:";
        private static string _slackWebhookUrl = Environment.GetEnvironmentVariable("SlackIncomingWebhookUrl");

        [FunctionName("CSharpCompilerSlackOuthookCSharp")]
        public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(CSharpCompilerSlackOuthookCSharp)} : Webhook was triggered!");

            string content = await req.Content.ReadAsStringAsync();
            log.Info(content);

            var data = content
                .Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => WebUtility.HtmlDecode(WebUtility.UrlDecode(x[1])));

            if (data["user_name"] == "slackbot")
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    body = "Cannot Support Messages From SlackBot.",
                });
            }

            var text = data["text"] as string ?? "";
            log.Info(text);

            var code = text.Replace(TRIGGER_WORD, "");

            // Evaluate C# Code with Roslyn
            log.Info($"{nameof(code)} : {code}");
            var resultText = await RoslynCompiler.EvaluateCSharpAsync(code);
            log.Info(resultText);

            // Send back with Slack Incoming Webhook
            var message = string.IsNullOrWhiteSpace(resultText) ? "‹ó‚¾ƒjƒƒ" : resultText;
            var payload = new
            {
                channel = "#azurefunctions",
                username = "C# Evaluator",
                text = message,
                icon_url = "https://azure.microsoft.com/svghandler/visual-studio-team-services/?width=300&height=300",
            };

            var jsonString = JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(_slackWebhookUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                return req.CreateResponse(res.StatusCode, new
                {
                    body = $"CSharp Evaluate message. Message : {message}",
                });
            }
        }
    }
}
