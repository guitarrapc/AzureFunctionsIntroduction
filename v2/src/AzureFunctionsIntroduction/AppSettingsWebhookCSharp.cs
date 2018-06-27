using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;

namespace AzureFunctionsIntroduction
{
    public static class AppSettingsWebhookCSharp
    {
        [FunctionName("AppSettingsWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(AppSettingsWebhookCSharp)} : C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Input>(jsonContent);

            // You can access Azure Functions Portal > Application Settings setting variable.
            var envKey = data.key;
            var envValue = ConfigurationManagerHelper.GetOrDefault(envKey);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                key = envKey,
                value = envValue,
            });
        }

        private class Input
        {
            public string key { get; set; }
        }
    }
}
