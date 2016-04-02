#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    var jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    
    if (data.comment == null || data.comment.body == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass comment:body properties in the input object"
        });
    }
    log.Verbose($"GitHub WebHook triggered!, {data}");
    var message = $@"New GitHub comment posted by {data.comment.user.login} at {data.repository.name},
        Url : {data.comment.url}
        Tite : {data.issue.title}
        -----
        {data.comment.body}";
        
    var webhookUrl = "Input your Slack Notification Azure Functions Url";
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
        return req.CreateResponse(res.StatusCode, new {
            body = $"VSTS Build message. Message : {message}",
        });
    }
}