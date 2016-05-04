#load "..\EnumerableExtensions.csx"

#r "System.Web"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

private const string TRIGGER_WORD = "@C#:";
private const string SlackWebhookUrl = "Input your Slack Notification Azure Functions Url";

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Verbose("Outgoding webhook Charp Compiler service was triggered!");

    var content = await req.Content.ReadAsStringAsync();
    log.Verbose(content);

    var data = content
        .Split('&')
        .Select(x => x.Split('='))
        .ToDictionary(x => x[0], x => HttpUtility.HtmlDecode(HttpUtility.UrlDecode(x[1])));

    if (data["user_name"] == "slackbot")
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            body = "Cannot Support Messages From SlackBot.",
        });
    }

    var text = data["text"] as string ?? "";
    log.Verbose(text);

    var code = text.Replace(TRIGGER_WORD, "");

    // Evaluate C# Code with Roslyn
    log.Verbose($"{nameof(code)} : {code}");
    var resultText = await EvaluateCSharpAsync(code);
    log.Verbose(resultText);

    // デバッグ用
    log.Verbose(res.StatusCode.ToString());
    log.Verbose(res.Error);

    // Send back with Slack Incoming Webhook
    var message = string.IsNullOrWhiteSpace(resultText) ? "空だニャ" : resultText;
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
        var res = await client.PostAsync(SlackWebhookUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"));
        return req.CreateResponse(res.StatusCode, new {
            body = $"CSharp Evaluate message. Message : {message}",
        });
    }
}