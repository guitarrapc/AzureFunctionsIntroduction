using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;
using CSharpScripting;
using System.Net.Http;

namespace CSharpCompilerWebhookCSharp
{
    public class FunctionTrigger
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Charp Compiler service Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data.code == null)
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