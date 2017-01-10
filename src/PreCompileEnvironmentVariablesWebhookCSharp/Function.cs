using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace PreCompileEnvironmentVariablesWebhookCSharp
{
    public class Function
    {
        private static readonly TraceWriter log = new CustomTraceWriter(System.Diagnostics.TraceLevel.Info);

        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req)
        {
            log.Info($"Webhook was triggered!");

            var appKey = "FooKey";
            var appValue = Environment.GetEnvironmentVariable(appKey);
            log.Info($"App Setting. Key : {appKey}, Value : {appValue}");

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
                greeting = $"Hello {data.first} {data.last}!"
            });
        }
    }
}
