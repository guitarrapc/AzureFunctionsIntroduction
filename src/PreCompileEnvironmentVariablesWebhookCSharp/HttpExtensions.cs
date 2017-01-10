using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace PreCompileEnvironmentVariablesWebhookCSharp
{
    public static class HttpExtensions
    {
        private static readonly TraceWriter log = new CustomTraceWriter(System.Diagnostics.TraceLevel.Info);

        public static HttpResponseMessage CreateResponse(this HttpRequestMessage req, HttpStatusCode statusCode, Object message)
        {
            log.Info(message.ToString());
            return new HttpResponseMessage(statusCode);
        }
    }
}
