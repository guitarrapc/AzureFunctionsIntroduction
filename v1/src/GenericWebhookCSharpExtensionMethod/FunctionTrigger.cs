using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace GenericWebhookCSharpExtensionMethod
{
    public class FunctionTrigger
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.first == null || data.last == null) {
                return req.CreateResponse(HttpStatusCode.BadRequest, new {
                    error = "Please pass first/last properties in the input object"
                });
            }

            var extensionMethodTest = Enumerable.Range(1, 11).Select(x => x * 10).ToArray().ToJoinedString(",");
            return req.CreateResponse(HttpStatusCode.OK, new {
                greeting = $"Hello {data.first} {data.last} and {extensionMethodTest}!"
            });
        }
    }

    public static class EnumerableExtensions
    {
        public static string ToJoinedString<T>(this IEnumerable<T> source, string separator = "")
        {
            return string.Join(separator, source);
        }
    }
}