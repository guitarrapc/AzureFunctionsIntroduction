using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AppSettingsWebhookCSharp
{
    public class FunctionTrigger
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"AppSettingsWebhookCSharp. C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

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

            if (data.first == null || data.last == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass first/last properties in the input object"
                });
            }

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                greeting = $"Hello {nameof(appKey)} : {appKey}, {nameof(appValue)} : {appValue}!"
            });
        }
    }
}