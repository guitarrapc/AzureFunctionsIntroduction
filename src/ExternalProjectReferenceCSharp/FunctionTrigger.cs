using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using NuGetSample;
using MyExtensions;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ExternalProjectReferenceCSharp
{
    public class FunctionTrigger
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.first == null || data.last == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "Please pass first/last properties in the input object"
                });
            }

            // Load .csx from same directory
            var hoge = TestMethod.Test();
            log.Info(hoge);

            // Load .csx from same directory + pass TraceWriter
            TestMethod.Test2(log);

            // Load .csx from parent directory
            var concat = Enumerable.Range(1, 10).Select(x => x * x).ToArray().ToJoinedString(",");
            log.Info(concat);

            // Load NuGet .csx test
            var fuga = OctokitSample.NugetTest();
            log.Info(fuga);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                greeting = $"Hello {data.first} {data.last}!"
            });
        }
    }
}