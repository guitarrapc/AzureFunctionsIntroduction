using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Utf8Json;

namespace AzureFunctionsIntroduction
{
    public static class AppSettingsWebhookCSharp
    {
        [FunctionName("AppSettingsWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation($"{nameof(AppSettingsWebhookCSharp)} : C# HTTP trigger function processed a request.");

            switch (req.Method.Method)
            {
                case "POST":
                    return await PostHandler(req);
                case "GET":
                    return await GetHandler(req);
                default:
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Unexpected request type detected.");
            }
        }

        private static async Task<HttpResponseMessage> PostHandler(HttpRequestMessage req)
        {
            var data = await req.Content.ReadAsAsync<Input>();

            if (data == null)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Required post data 'key' not found.");
            }

            // You can access Azure Functions Portal > Application Settings setting variable.
            var envKey = data.key;
            var envValue = EnvironmentHelper.GetOrDefault(envKey, "");

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                key = envKey,
                value = envValue,
            });
        }

        private static async Task<HttpResponseMessage> GetHandler(HttpRequestMessage req)
        {
            var input = new Input();
            var keyValues = req.GetQueryNameValuePairs().Where(x => string.Equals(x.Key, input.key, StringComparison.OrdinalIgnoreCase));
            if (!keyValues.Any())
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Required query parameter 'key' not found.");
            }

            // only check first key
            var envKey = keyValues.First().Value;
            // You can access Azure Functions Portal > Application Settings setting variable.
            var envValue = EnvironmentHelper.GetOrDefault(envKey, "");

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
