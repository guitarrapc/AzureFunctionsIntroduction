#load "..\CSharpScripting.csx"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Verbose("Charp Compiler service Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    
    // CSharp Code評価
    string code = data.code;
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