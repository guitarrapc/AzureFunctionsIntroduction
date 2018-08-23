using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using AzureFunctionsIntroduction.Features.Roslyn;

namespace AzureFunctionsIntroduction
{
    public static class CSharpCompilerWebhookCSharp
    {
        [FunctionName("CSharpCompilerWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"{nameof(CSharpCompilerWebhookCSharp)} : C# HTTP trigger function processed a request.");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data == null || data.code == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "missing <code> property."
                });
            }

            // Evaluate CSharp Code
            string code = data.code;
            log.Info($"{nameof(code)} : {code}");
            var resultText = await RoslynCompiler.EvaluateCSharpAsync(code);
            log.Info(resultText);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                body = resultText,
            });
        }
    }
}