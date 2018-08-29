using AzureFunctionsIntroduction.Features;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction
{
    // request : {"code" : "Enumerable.Range(0, 10).Select(x => x * x).Sum()"}
    // should be : {"body":"285"}
    public class RoslynCompilerServiceCSharp
    {
        [FunctionName("CSharpCompilerWebhookCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"CSharpCompilerWebhookCSharp : C# HTTP trigger function processed a request.");

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
            var resultText = await RoslynComplier.EvaluateCSharpAsync(code);
            log.Info(resultText);

            return req.CreateResponse(HttpStatusCode.OK, new
            {
                body = resultText,
            });
        }
    }
}
