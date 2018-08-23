using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzureFunctionsIntroduction
{
    public static class GenericWebhookCSharpExtensionMethod
    {
        [FunctionName("GenericWebhookCSharpExtensionMethod")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(GenericWebhookCSharpExtensionMethod)} : C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic input = JsonConvert.DeserializeObject(jsonContent);
            var start = int.Parse((string)input.start);
            var range = int.Parse((string)input.range);

            var extensionMethodTest = Enumerable.Range(start, range).Select(x => x * 10).ToArray().ToJoinedString(",");
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                Result = $"{extensionMethodTest}"
            });
        }
    }
}
