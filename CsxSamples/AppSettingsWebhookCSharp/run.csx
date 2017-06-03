#r "Newtonsoft.Json"

using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    // Both code is same meaning with AzureFunctions (Azure Web Apps).
    // System.Configuration.ConfigurationManager.AppSettings[Key];
    // System.Environment.GetEnvironmentVariable("Key");
    var appKey = "FooKey";
    var appValue = ConfigurationManager.AppSettings[appKey];
    log.Info($"App Setting. Key : {appKey}, Value : {appValue}");
    var envValue = Environment.GetEnvironmentVariable(appKey);
    log.Info($"Environment Setting. Key : {appKey}, Value : {envValue}");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.first == null || data.last == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass first/last properties in the input object"
        });
    }

    return req.CreateResponse(HttpStatusCode.OK, new {
        greeting = $"Hello {data.first} {data.last}!"
    });
}
